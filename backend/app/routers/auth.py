from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy import select
from sqlalchemy.ext.asyncio import AsyncSession

from app.database import get_db
from app.models.models import User, UserRole, Role, TenantProfile, LandlordProfile, AdminProfile
from app.schemas.schemas import UserCreate, UserLogin, TokenOut, UserOut
from app.services.auth import (
    hash_password,
    verify_password,
    create_access_token,
    get_user_by_email,
    decode_token,
)

router = APIRouter(prefix="/api/auth", tags=["auth"])


@router.post("/register", response_model=TokenOut)
async def register(data: UserCreate, db: AsyncSession = Depends(get_db)):
    existing = await get_user_by_email(db, data.email)
    if existing:
        raise HTTPException(status_code=400, detail="Email already registered")

    user = User(
        email=data.email,
        password_hash=hash_password(data.password),
        phone=data.phone,
    )
    db.add(user)
    await db.flush()

    role_result = await db.execute(select(Role).where(Role.role_name == data.role))
    role = role_result.scalar_one_or_none()
    if not role:
        role = Role(role_name=data.role)
        db.add(role)
        await db.flush()

    user_role = UserRole(user_id=user.user_id, role_id=role.role_id)
    db.add(user_role)

    if data.role == "tenant" and data.first_name and data.last_name:
        profile = TenantProfile(
            tenant_id=user.user_id,
            first_name=data.first_name,
            last_name=data.last_name,
        )
        db.add(profile)

    await db.commit()

    token = create_access_token({"sub": str(user.user_id), "email": user.email})
    return TokenOut(
        access_token=token,
        user_id=user.user_id,
        email=user.email,
        roles=[data.role],
    )


@router.post("/login", response_model=TokenOut)
async def login(data: UserLogin, db: AsyncSession = Depends(get_db)):
    user = await get_user_by_email(db, data.email)
    if not user or not verify_password(data.password, user.password_hash):
        raise HTTPException(status_code=401, detail="Invalid credentials")

    roles = [ur.role.role_name for ur in user.roles]
    token = create_access_token({"sub": str(user.user_id), "email": user.email})

    from datetime import datetime
    user.last_login_date = datetime.utcnow()
    await db.commit()

    return TokenOut(
        access_token=token,
        user_id=user.user_id,
        email=user.email,
        roles=roles,
    )


@router.get("/me", response_model=UserOut)
async def get_me(token: str, db: AsyncSession = Depends(get_db)):
    payload = decode_token(token)
    if not payload:
        raise HTTPException(status_code=401, detail="Invalid token")

    from app.services.auth import get_user_by_id
    user = await get_user_by_id(db, int(payload["sub"]))
    if not user:
        raise HTTPException(status_code=404, detail="User not found")

    roles = [ur.role.role_name for ur in user.roles]
    return UserOut(
        user_id=user.user_id,
        email=user.email,
        phone=user.phone,
        registration_date=user.registration_date,
        is_active=user.is_active,
        roles=roles,
    )
