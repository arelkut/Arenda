# Недвижимость.РФ

Платформа для поиска и аренды недвижимости по всей России.

## Структура проекта

```
├── backend/          # FastAPI REST API
│   ├── app/
│   │   ├── models/   # SQLAlchemy модели (MSSQL)
│   │   ├── schemas/  # Pydantic схемы
│   │   ├── routers/  # API эндпоинты
│   │   └── services/ # Бизнес-логика
│   └── pyproject.toml
├── frontend/         # React + Vite + TypeScript
│   ├── src/
│   │   ├── components/  # Общие компоненты (Header, Footer)
│   │   ├── pages/       # Страницы
│   │   ├── context/     # React Context (AuthContext)
│   │   └── services/    # API клиент
│   └── package.json
└── database/         # SQL-скрипты для SSMS
    └── RpmDb_SSMS.sql
```

## Технологии

- **Frontend**: React 19, TypeScript, Vite, React Router, Lucide Icons
- **Backend**: Python 3.12, FastAPI, SQLAlchemy 2.0, aioodbc
- **База данных**: MS SQL Server (SSMS совместимый)
- **Авторизация**: JWT (python-jose + passlib/bcrypt)

## Страницы сайта

1. **Главная** — Landing page с hero секцией, карточками услуг, типами недвижимости, объявлениями, статистикой
2. **Каталог** — Каталог объявлений с фильтрами и поиском
3. **Ипотека** — Калькулятор ипотеки + предложения банков
4. **Блог** — Статьи о недвижимости
5. **Аналитика** — Статистика и тренды рынка
6. **Отзывы** — Отзывы клиентов
7. **Авторизация** — Вход / Регистрация / Восстановление пароля

## Запуск

### База данных (MSSQL)

1. Откройте SSMS и подключитесь к серверу
2. Запустите скрипт `database/RpmDb_SSMS.sql`

### Backend

```bash
cd backend
pip install -e .
# Настройте DATABASE_URL в .env или переменных окружения
uvicorn app.main:app --reload --port 8000
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Сайт будет доступен на `http://localhost:5173`

## API

Документация API доступна по адресу `http://localhost:8000/docs` (Swagger UI).

### Основные эндпоинты

- `POST /api/auth/register` — Регистрация
- `POST /api/auth/login` — Авторизация
- `GET /api/properties` — Список объявлений
- `GET /api/properties/{id}` — Детали объявления
- `GET /api/health` — Проверка состояния API

## Демо-аккаунты

При первом запуске backend автоматически создаёт:
- **Админ**: admin@example.com / admin123
- **Арендодатель**: realtor@example.com / realtor123
- **Арендатор**: client@example.com / client123
