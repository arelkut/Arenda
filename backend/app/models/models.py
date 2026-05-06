from datetime import date, datetime
from decimal import Decimal

from sqlalchemy import (
    Boolean,
    Column,
    Date,
    DateTime,
    ForeignKey,
    Integer,
    Numeric,
    Text,
    Unicode,
    UnicodeText,
)
from sqlalchemy.orm import relationship

from app.database import Base


class Role(Base):
    __tablename__ = "roles"

    role_id = Column(Integer, primary_key=True, autoincrement=True)
    role_name = Column(Unicode(50), unique=True, nullable=False)

    users = relationship("UserRole", back_populates="role")


class User(Base):
    __tablename__ = "users"

    user_id = Column(Integer, primary_key=True, autoincrement=True)
    email = Column(Unicode(100), unique=True, nullable=False)
    password_hash = Column(Unicode(255), nullable=False)
    phone = Column(Unicode(20), nullable=True)
    registration_date = Column(DateTime, default=datetime.utcnow)
    is_active = Column(Boolean, default=True)
    last_login_date = Column(DateTime, nullable=True)

    roles = relationship("UserRole", back_populates="user", cascade="all, delete-orphan")
    admin_profile = relationship("AdminProfile", back_populates="user", uselist=False, cascade="all, delete-orphan")
    landlord_profile = relationship("LandlordProfile", back_populates="user", uselist=False, cascade="all, delete-orphan")
    tenant_profile = relationship("TenantProfile", back_populates="user", uselist=False, cascade="all, delete-orphan")
    favorites = relationship("Favorite", back_populates="user")
    notifications = relationship("Notification", back_populates="user", cascade="all, delete-orphan")


class UserRole(Base):
    __tablename__ = "user_roles"

    user_id = Column(Integer, ForeignKey("users.user_id", ondelete="CASCADE"), primary_key=True)
    role_id = Column(Integer, ForeignKey("roles.role_id"), primary_key=True)

    user = relationship("User", back_populates="roles")
    role = relationship("Role", back_populates="users")


class AdminProfile(Base):
    __tablename__ = "admin_profiles"

    admin_id = Column(Integer, ForeignKey("users.user_id", ondelete="CASCADE"), primary_key=True)
    full_name = Column(Unicode(100), nullable=False)

    user = relationship("User", back_populates="admin_profile")


class LandlordProfile(Base):
    __tablename__ = "landlord_profiles"

    landlord_id = Column(Integer, ForeignKey("users.user_id", ondelete="CASCADE"), primary_key=True)
    company_name = Column(Unicode(100), nullable=True)
    contact_person = Column(Unicode(100), nullable=False)
    inn = Column(Unicode(20), nullable=True)

    user = relationship("User", back_populates="landlord_profile")


class TenantProfile(Base):
    __tablename__ = "tenant_profiles"

    tenant_id = Column(Integer, ForeignKey("users.user_id", ondelete="CASCADE"), primary_key=True)
    first_name = Column(Unicode(50), nullable=False)
    last_name = Column(Unicode(50), nullable=False)
    passport_data = Column(Unicode(100), nullable=True)

    user = relationship("User", back_populates="tenant_profile")


class PropertyStatus(Base):
    __tablename__ = "property_statuses"

    status_id = Column(Integer, primary_key=True, autoincrement=True)
    status_name = Column(Unicode(50), unique=True, nullable=False)


class Property(Base):
    __tablename__ = "properties"

    property_id = Column(Integer, primary_key=True, autoincrement=True)
    landlord_id = Column(Integer, ForeignKey("users.user_id"), nullable=False)
    title = Column(Unicode(200), nullable=False)
    description = Column(UnicodeText, nullable=True)
    address = Column(Unicode(255), nullable=False)
    city = Column(Unicode(100), nullable=False)
    district = Column(Unicode(100), nullable=True)
    property_type = Column(Unicode(50), nullable=False)
    area = Column(Numeric(10, 2), nullable=False)
    rooms = Column(Integer, nullable=True)
    floor = Column(Integer, nullable=True)
    total_floors = Column(Integer, nullable=True)
    price = Column(Numeric(19, 2), nullable=False)
    is_active = Column(Boolean, default=False)
    created_date = Column(DateTime, default=datetime.utcnow)
    status_id = Column(Integer, ForeignKey("property_statuses.status_id"), default=1)

    landlord = relationship("User")
    status = relationship("PropertyStatus")
    media = relationship("PropertyMedia", back_populates="property", cascade="all, delete-orphan")
    favorites = relationship("Favorite", back_populates="property")


class PropertyMedia(Base):
    __tablename__ = "property_media"

    media_id = Column(Integer, primary_key=True, autoincrement=True)
    property_id = Column(Integer, ForeignKey("properties.property_id", ondelete="CASCADE"), nullable=False)
    media_type = Column(Unicode(10), nullable=False)
    file_path = Column(Unicode(500), nullable=False)
    is_main = Column(Boolean, default=False)
    upload_date = Column(DateTime, default=datetime.utcnow)

    property = relationship("Property", back_populates="media")


class Favorite(Base):
    __tablename__ = "favorites"

    user_id = Column(Integer, ForeignKey("users.user_id"), primary_key=True)
    property_id = Column(Integer, ForeignKey("properties.property_id"), primary_key=True)
    added_date = Column(DateTime, default=datetime.utcnow)

    user = relationship("User", back_populates="favorites")
    property = relationship("Property", back_populates="favorites")


class LeaseAgreement(Base):
    __tablename__ = "lease_agreements"

    agreement_id = Column(Integer, primary_key=True, autoincrement=True)
    tenant_id = Column(Integer, ForeignKey("users.user_id"), nullable=False)
    property_id = Column(Integer, ForeignKey("properties.property_id"), nullable=False)
    start_date = Column(Date, nullable=False)
    end_date = Column(Date, nullable=False)
    monthly_rent = Column(Numeric(19, 2), nullable=False)
    deposit = Column(Numeric(19, 2), nullable=True)
    document_path = Column(Unicode(500), nullable=True)
    is_active = Column(Boolean, default=True)
    created_date = Column(DateTime, default=datetime.utcnow)

    tenant = relationship("User")
    property = relationship("Property")
    payments = relationship("Payment", back_populates="agreement")
    transfer_acts = relationship("TransferAct", back_populates="agreement", cascade="all, delete-orphan")


class PaymentStatus(Base):
    __tablename__ = "payment_statuses"

    status_id = Column(Integer, primary_key=True, autoincrement=True)
    status_name = Column(Unicode(50), unique=True, nullable=False)


class Payment(Base):
    __tablename__ = "payments"

    payment_id = Column(Integer, primary_key=True, autoincrement=True)
    agreement_id = Column(Integer, ForeignKey("lease_agreements.agreement_id"), nullable=False)
    amount = Column(Numeric(19, 2), nullable=False)
    due_date = Column(Date, nullable=False)
    payment_date = Column(Date, nullable=True)
    status_id = Column(Integer, ForeignKey("payment_statuses.status_id"), default=1)
    transaction_id = Column(Unicode(100), nullable=True)

    agreement = relationship("LeaseAgreement", back_populates="payments")
    status = relationship("PaymentStatus")


class RequestStatus(Base):
    __tablename__ = "request_statuses"

    status_id = Column(Integer, primary_key=True, autoincrement=True)
    status_name = Column(Unicode(50), unique=True, nullable=False)


class ViewingRequest(Base):
    __tablename__ = "viewing_requests"

    request_id = Column(Integer, primary_key=True, autoincrement=True)
    tenant_id = Column(Integer, ForeignKey("users.user_id"), nullable=False)
    property_id = Column(Integer, ForeignKey("properties.property_id"), nullable=False)
    request_date = Column(DateTime, default=datetime.utcnow)
    preferred_date = Column(DateTime, nullable=True)
    message = Column(Unicode(500), nullable=True)
    status_id = Column(Integer, ForeignKey("request_statuses.status_id"), default=1)

    tenant = relationship("User")
    property = relationship("Property")
    status = relationship("RequestStatus")


class Notification(Base):
    __tablename__ = "notifications"

    notification_id = Column(Integer, primary_key=True, autoincrement=True)
    user_id = Column(Integer, ForeignKey("users.user_id", ondelete="CASCADE"), nullable=False)
    message = Column(Unicode(500), nullable=False)
    notification_type = Column(Unicode(50), nullable=False)
    is_read = Column(Boolean, default=False)
    created_date = Column(DateTime, default=datetime.utcnow)

    user = relationship("User", back_populates="notifications")


class ModerationHistory(Base):
    __tablename__ = "moderation_history"

    moderation_id = Column(Integer, primary_key=True, autoincrement=True)
    property_id = Column(Integer, ForeignKey("properties.property_id"), nullable=False)
    admin_id = Column(Integer, ForeignKey("users.user_id"), nullable=False)
    action = Column(Unicode(20), nullable=False)
    comment = Column(Unicode(500), nullable=True)
    action_date = Column(DateTime, default=datetime.utcnow)

    property = relationship("Property")
    admin = relationship("User")


class TransferAct(Base):
    __tablename__ = "transfer_acts"

    act_id = Column(Integer, primary_key=True, autoincrement=True)
    agreement_id = Column(Integer, ForeignKey("lease_agreements.agreement_id", ondelete="CASCADE"), nullable=False)
    act_type = Column(Unicode(20), nullable=False)
    act_date = Column(Date, nullable=False)
    document_path = Column(Unicode(500), nullable=True)
    notes = Column(UnicodeText, nullable=True)

    agreement = relationship("LeaseAgreement", back_populates="transfer_acts")
