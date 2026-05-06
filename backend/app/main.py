from contextlib import asynccontextmanager

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.database import engine, Base
from app.models.models import (
    Role, User, UserRole, AdminProfile, LandlordProfile, TenantProfile,
    PropertyStatus, Property, PropertyMedia, Favorite, LeaseAgreement,
    PaymentStatus, Payment, RequestStatus, ViewingRequest,
    Notification, ModerationHistory, TransferAct,
)
from app.routers import auth, properties
from app.services.auth import hash_password


async def seed_data():
    from sqlalchemy.ext.asyncio import AsyncSession
    from app.database import async_session
    from sqlalchemy import select

    async with async_session() as db:
        existing = await db.execute(select(Role))
        if existing.scalars().first():
            return

        roles = [
            Role(role_name="admin"),
            Role(role_name="landlord"),
            Role(role_name="tenant"),
        ]
        db.add_all(roles)
        await db.flush()

        prop_statuses = [
            PropertyStatus(status_name="На модерации"),
            PropertyStatus(status_name="Активно"),
            PropertyStatus(status_name="Отклонено"),
            PropertyStatus(status_name="Архив"),
        ]
        db.add_all(prop_statuses)

        payment_statuses = [
            PaymentStatus(status_name="Ожидает оплаты"),
            PaymentStatus(status_name="Оплачено"),
            PaymentStatus(status_name="Просрочено"),
        ]
        db.add_all(payment_statuses)

        request_statuses = [
            RequestStatus(status_name="Новая"),
            RequestStatus(status_name="Подтверждена"),
            RequestStatus(status_name="Отклонена"),
            RequestStatus(status_name="Завершена"),
        ]
        db.add_all(request_statuses)
        await db.flush()

        admin = User(email="admin@example.com", password_hash=hash_password("admin123"), phone="+79001234567")
        landlord = User(email="realtor@example.com", password_hash=hash_password("realtor123"), phone="+79001234568")
        tenant = User(email="client@example.com", password_hash=hash_password("client123"), phone="+79001234569")
        db.add_all([admin, landlord, tenant])
        await db.flush()

        db.add_all([
            UserRole(user_id=admin.user_id, role_id=roles[0].role_id),
            UserRole(user_id=landlord.user_id, role_id=roles[1].role_id),
            UserRole(user_id=tenant.user_id, role_id=roles[2].role_id),
        ])

        db.add(AdminProfile(admin_id=admin.user_id, full_name="Администратор Системы"))
        db.add(LandlordProfile(landlord_id=landlord.user_id, company_name="РиэлтПро", contact_person="Иван Иванов", inn="1234567890"))
        db.add(TenantProfile(tenant_id=tenant.user_id, first_name="Пётр", last_name="Петров"))

        active_status = [s for s in prop_statuses if s.status_name == "Активно"][0]
        await db.flush()

        sample_properties = [
            Property(
                landlord_id=landlord.user_id, title="Уютная 2-комнатная квартира в центре",
                description="Светлая квартира с современным ремонтом, рядом метро и парк.",
                address="ул. Пушкина, д. 10, кв. 25", city="Москва", district="Центральный",
                property_type="Квартира", area=65.5, rooms=2, floor=5, total_floors=9,
                price=85000, is_active=True, status_id=active_status.status_id,
            ),
            Property(
                landlord_id=landlord.user_id, title="Просторная студия с панорамным видом",
                description="Новая студия в современном жилом комплексе с видом на город.",
                address="пр. Ленина, д. 45, кв. 120", city="Москва", district="Северный",
                property_type="Студия", area=38.0, rooms=1, floor=15, total_floors=25,
                price=55000, is_active=True, status_id=active_status.status_id,
            ),
            Property(
                landlord_id=landlord.user_id, title="3-комнатная квартира с ремонтом",
                description="Семейная квартира в тихом районе, рядом школа и детский сад.",
                address="ул. Гагарина, д. 22, кв. 8", city="Санкт-Петербург", district="Приморский",
                property_type="Квартира", area=90.0, rooms=3, floor=3, total_floors=5,
                price=120000, is_active=True, status_id=active_status.status_id,
            ),
            Property(
                landlord_id=landlord.user_id, title="Коммерческое помещение на первом этаже",
                description="Идеально для офиса или магазина, высокий трафик, витринные окна.",
                address="ул. Мира, д. 5", city="Москва", district="Центральный",
                property_type="Коммерческая", area=120.0, rooms=None, floor=1, total_floors=12,
                price=250000, is_active=True, status_id=active_status.status_id,
            ),
            Property(
                landlord_id=landlord.user_id, title="Загородный дом с участком",
                description="Кирпичный дом с гаражом, баней и ухоженным садом в 20 км от города.",
                address="пос. Зелёное, ул. Сосновая, д. 12", city="Московская область", district="Одинцовский",
                property_type="Дом", area=180.0, rooms=5, floor=2, total_floors=2,
                price=200000, is_active=True, status_id=active_status.status_id,
            ),
            Property(
                landlord_id=landlord.user_id, title="1-комнатная квартира рядом с метро",
                description="Чистая квартира с хорошим транспортным сообщением.",
                address="ул. Советская, д. 33, кв. 14", city="Москва", district="Восточный",
                property_type="Квартира", area=42.0, rooms=1, floor=7, total_floors=16,
                price=45000, is_active=True, status_id=active_status.status_id,
            ),
        ]
        db.add_all(sample_properties)
        await db.commit()


@asynccontextmanager
async def lifespan(app: FastAPI):
    async with engine.begin() as conn:
        await conn.run_sync(Base.metadata.create_all)
    await seed_data()
    yield


app = FastAPI(title="Недвижимость.РФ API", version="1.0.0", lifespan=lifespan)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(auth.router)
app.include_router(properties.router)


@app.get("/api/health")
async def health():
    return {"status": "ok"}
