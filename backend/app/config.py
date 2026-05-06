from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    DATABASE_URL: str = "mssql+aioodbc://sa:RpmStr0ng!Pass@localhost:1433/RpmDb?driver=ODBC+Driver+18+for+SQL+Server&TrustServerCertificate=yes"
    SECRET_KEY: str = "supersecretkey-change-in-production"
    ALGORITHM: str = "HS256"
    ACCESS_TOKEN_EXPIRE_MINUTES: int = 60

    class Config:
        env_file = ".env"


settings = Settings()
