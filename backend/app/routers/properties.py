from typing import Optional

from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy import select, func
from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy.orm import selectinload

from app.database import get_db
from app.models.models import Property, PropertyMedia, Favorite
from app.schemas.schemas import PropertyCreate, PropertyOut, PropertyListOut, PropertyMediaOut

router = APIRouter(prefix="/api/properties", tags=["properties"])


@router.get("", response_model=PropertyListOut)
async def list_properties(
    page: int = Query(1, ge=1),
    size: int = Query(12, ge=1, le=50),
    city: Optional[str] = None,
    property_type: Optional[str] = None,
    min_price: Optional[float] = None,
    max_price: Optional[float] = None,
    rooms: Optional[int] = None,
    search: Optional[str] = None,
    db: AsyncSession = Depends(get_db),
):
    query = select(Property).options(selectinload(Property.media)).where(Property.is_active == True)

    if city:
        query = query.where(Property.city.ilike(f"%{city}%"))
    if property_type:
        query = query.where(Property.property_type == property_type)
    if min_price is not None:
        query = query.where(Property.price >= min_price)
    if max_price is not None:
        query = query.where(Property.price <= max_price)
    if rooms is not None:
        query = query.where(Property.rooms == rooms)
    if search:
        query = query.where(
            Property.title.ilike(f"%{search}%") | Property.address.ilike(f"%{search}%")
        )

    count_query = select(func.count()).select_from(query.subquery())
    total_result = await db.execute(count_query)
    total = total_result.scalar()

    query = query.order_by(Property.created_date.desc()).offset((page - 1) * size).limit(size)
    result = await db.execute(query)
    items = result.scalars().all()

    return PropertyListOut(
        items=[PropertyOut.model_validate(p) for p in items],
        total=total,
        page=page,
        size=size,
    )


@router.get("/{property_id}", response_model=PropertyOut)
async def get_property(property_id: int, db: AsyncSession = Depends(get_db)):
    result = await db.execute(
        select(Property)
        .options(selectinload(Property.media))
        .where(Property.property_id == property_id)
    )
    prop = result.scalar_one_or_none()
    if not prop:
        raise HTTPException(status_code=404, detail="Property not found")
    return PropertyOut.model_validate(prop)


@router.post("", response_model=PropertyOut)
async def create_property(data: PropertyCreate, landlord_id: int, db: AsyncSession = Depends(get_db)):
    prop = Property(
        landlord_id=landlord_id,
        title=data.title,
        description=data.description,
        address=data.address,
        city=data.city,
        district=data.district,
        property_type=data.property_type,
        area=data.area,
        rooms=data.rooms,
        floor=data.floor,
        total_floors=data.total_floors,
        price=data.price,
    )
    db.add(prop)
    await db.commit()
    await db.refresh(prop)

    result = await db.execute(
        select(Property)
        .options(selectinload(Property.media))
        .where(Property.property_id == prop.property_id)
    )
    prop = result.scalar_one()
    return PropertyOut.model_validate(prop)
