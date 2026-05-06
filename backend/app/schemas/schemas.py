from datetime import date, datetime
from decimal import Decimal
from typing import Optional

from pydantic import BaseModel, EmailStr


class RoleOut(BaseModel):
    role_id: int
    role_name: str

    class Config:
        from_attributes = True


class UserCreate(BaseModel):
    email: str
    password: str
    phone: Optional[str] = None
    first_name: Optional[str] = None
    last_name: Optional[str] = None
    role: str = "tenant"


class UserLogin(BaseModel):
    email: str
    password: str


class TokenOut(BaseModel):
    access_token: str
    token_type: str = "bearer"
    user_id: int
    email: str
    roles: list[str]


class UserOut(BaseModel):
    user_id: int
    email: str
    phone: Optional[str] = None
    registration_date: Optional[datetime] = None
    is_active: bool
    roles: list[str] = []

    class Config:
        from_attributes = True


class PropertyMediaOut(BaseModel):
    media_id: int
    media_type: str
    file_path: str
    is_main: bool

    class Config:
        from_attributes = True


class PropertyCreate(BaseModel):
    title: str
    description: Optional[str] = None
    address: str
    city: str
    district: Optional[str] = None
    property_type: str
    area: Decimal
    rooms: Optional[int] = None
    floor: Optional[int] = None
    total_floors: Optional[int] = None
    price: Decimal


class PropertyOut(BaseModel):
    property_id: int
    landlord_id: int
    title: str
    description: Optional[str] = None
    address: str
    city: str
    district: Optional[str] = None
    property_type: str
    area: Decimal
    rooms: Optional[int] = None
    floor: Optional[int] = None
    total_floors: Optional[int] = None
    price: Decimal
    is_active: bool
    created_date: Optional[datetime] = None
    status_id: int
    media: list[PropertyMediaOut] = []

    class Config:
        from_attributes = True


class PropertyListOut(BaseModel):
    items: list[PropertyOut]
    total: int
    page: int
    size: int


class FavoriteOut(BaseModel):
    property_id: int
    added_date: Optional[datetime] = None

    class Config:
        from_attributes = True


class ViewingRequestCreate(BaseModel):
    property_id: int
    preferred_date: Optional[datetime] = None
    message: Optional[str] = None


class ViewingRequestOut(BaseModel):
    request_id: int
    tenant_id: int
    property_id: int
    request_date: Optional[datetime] = None
    preferred_date: Optional[datetime] = None
    message: Optional[str] = None
    status_id: int

    class Config:
        from_attributes = True


class PasswordResetRequest(BaseModel):
    email: str


class PasswordResetConfirm(BaseModel):
    token: str
    new_password: str


class NotificationOut(BaseModel):
    notification_id: int
    message: str
    notification_type: str
    is_read: bool
    created_date: Optional[datetime] = None

    class Config:
        from_attributes = True
