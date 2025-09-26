# Flight Information System

Система для зберігання й пошуку інформації про авіарейси, що складається з двох підсистем:
- **FlightStorageService** – бекенд (Web API) для зберігання та надання даних про рейси
- **FlightClientApp** – фронтенд з UI для пошуку рейсів через REST API

## 📋 Технології

- **.NET 9** (C#)
- **ASP.NET Core Web API** / **Razor Pages**
- **PostgreSQL**
- **ADO.NET** з **Npgsql** (без ORM)
- **Bootstrap** для UI
- **Swagger** для документації API

## 🚀 Інструкція для запуску

### 1. Підготовка бази даних

1. Запустіть **PostgreSQL** (локально або в Docker)
2. Створіть базу даних `FlightsDb`:

```bash
# Підключіться до PostgreSQL
psql -U postgres

# Створіть базу даних
CREATE DATABASE "FlightsDb";

# Підключіться до нової бази
\c "FlightsDb"

# Виконайте скрипт створення функцій
\i server.sql
```

Або через **pgAdmin**:
- Створіть нову базу даних `FlightsDb`
- Відкрийте файл `server.sql`
- Виконайте скрипт

### 2. Налаштування підключення до БД

Перевірте connection string у файлі `FlightStorageService/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FlightsDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**Для Windows Authentication:** залиште `Trusted_Connection=true`  
**Для SQL Server Authentication:** змініте на:
```json
"DefaultConnection": "Server=localhost;Database=FlightsDb;User Id=your_username;Password=your_password;TrustServerCertificate=true;"
```

### 3. Запуск FlightStorageService (API)

```bash
cd FlightStorageService
dotnet restore
dotnet run
```

API буде доступне на: `http://localhost:5228`  
Swagger UI: `http://localhost:5228/swagger`

### 4. Запуск FlightClientApp (Web UI)

**В новому терміналі:**

```bash
cd FlightClientApp
dotnet restore
dotnet run
```

Web додаток буде доступний на: `http://localhost:5000`

## 🧪 Тестові дані

Для тестування додайте кілька рейсів через API або SQL:

```sql
-- Підключіться до бази FlightsDb
\c "FlightsDb"

-- Додайте тестові рейси
SELECT add_flight('AB123', '2025-09-27 10:30:00', 'Rome', 'Perechyn', 120);
SELECT add_flight('CD456', '2025-09-27 14:15:00', 'Perechyn', 'Warsaw', 90);
SELECT add_flight('EF789', '2025-09-28 08:00:00', 'Kyiv', 'London', 180);
```

## 📊 API Endpoints

| Метод | URL | Опис |
|-------|-----|------|
| `GET` | `/api/flights/{flightNumber}` | Пошук рейсу за номером |
| `GET` | `/api/flights?date={yyyy-MM-dd}` | Всі рейси на задану дату |
| `GET` | `/api/flights/departure?city={city}&date={yyyy-MM-dd}` | Рейси з міста вильоту |
| `GET` | `/api/flights/arrival?city={city}&date={yyyy-MM-dd}` | Рейси до міста прильоту |

### Приклади запитів:

```bash
# Пошук рейсу за номером
curl http://localhost:5228/api/flights/AB123

# Всі рейси на дату
curl "http://localhost:5228/api/flights?date=2025-09-27"

# Рейси з Rome на 27.09.2025
curl "http://localhost:5228/api/flights/departure?city=Rome&date=2025-09-27"

# Рейси до Perechyn на 27.09.2025
curl "http://localhost:5228/api/flights/arrival?city=Perechyn&date=2025-09-27"
```

## 🖥️ Використання Web UI

1. Відкрийте `http://localhost:5000/Flights`
2. Оберіть тип пошуку:
   - **За номером рейсу** - введіть номер (наприклад, AB123)
   - **За датою** - оберіть дату
   - **За містом вильоту** - введіть місто + дату
   - **За містом прильоту** - введіть місто + дату
3. Натисніть кнопку пошуку
4. Перегляньте результати в таблиці

## 🏗️ Архітектура проекту

```
FlightInformationSystem/
├── FlightStorageService/          # Web API
│   ├── Controllers/
│   │   └── FlightsController.cs
│   ├── Models/
│   │   └── Flight.cs
│   ├── Repositories/
│   │   └── FlightRepository.cs
│   ├── Services/
│   │   └── FlightService.cs
|   ├── server.sql  
│   └── Program.cs
├── FlightClientApp/               # Web UI
│   ├── Pages/
│   │   └── Flights.cshtml
│   │   └── FlightsModel.cs
│   └── Program.cs          
└── README.md
```

## 🔧 Налаштування для різних середовищ

### Docker (опціонально)

Якщо у вас є Docker, можете запустити PostgreSQL в контейнері:

```bash
docker run --name postgres-flights \
           -e POSTGRES_PASSWORD=mypassword \
           -e POSTGRES_DB=FlightsDb \
           -p 5432:5432 \
           postgres:15
```

І змінити connection string:
```json
"DefaultConnection": "Host=localhost;Port=5432;Database=FlightsDb;Username=postgres;Password=mypassword"
```

### Порти

За замовчуванням:
- **FlightStorageService**: `http://localhost:5228`
- **FlightClientApp**: `http://localhost:5000`

Щоб змінити порти, відредагуйте `Properties/launchSettings.json` у відповідних проектах.

## 🐛 Вирішення проблем

### Помилка підключення до БД
- Перевірте, чи запущений PostgreSQL
- Перевірте connection string
- Переконайтеся, що база `FlightsDb` створена
- Перевірте правильність username/password

### API недоступне
- Перевірте, чи запущений FlightStorageService
- Перевірте порт в URL (за замовчуванням 5228)
- Подивіться логи консолі на предмет помилок

### Проблеми з CORS
Якщо виникають CORS помилки, перевірте налаштування у `Program.cs` FlightStorageService.

## 🧪 Запуск тестів

```bash
cd FlightStorageService.Tests
dotnet test
```

## 📝 Обмеження

- Дані доступні лише на **найближчі 7 днів** від поточної дати
- Всі SQL операції виконуються через **PostgreSQL функції**
- Використовується тільки **ADO.NET з Npgsql** (без ORM)

## 👨‍💻 Автор

Створено як тестове завдання для позиції C# програміста.
