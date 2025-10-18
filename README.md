# Task Management System

Приложение для управления задачами с использованием Vue.js 3, ASP.NET Core 8, Firebird SQL и Dapper.

## Функциональность

- ✅ CRUD операции для задач
- ✅ Drag&Drop для изменения статуса задач
- ✅ Фильтрация и сортировка задач
- ✅ Валидация данных с FluentValidation
- ✅ RESTful API с чистой архитектурой
- ✅ Статистика по задачам
- ✅ Адаптивный интерфейс

## Технологии

### Backend
- ASP.NET Core 8.0
- Firebird SQL
- Dapper ORM
- FluentValidation
- Swagger для документации API

### Frontend
- Vue.js 3
- VueDraggable для Drag&Drop
- Axios для HTTP запросов
- Vite как сборщик

## Требования

- .NET SDK 8.0 или выше
- Node.js 20.0 или выше (для frontend)
- Firebird 5.0
- npm или yarn

## Установка и запуск

### 1. База данных

1. Установите Firebird SQL Server с официального сайта: https://firebirdsql.org/en/downloads/
2. Создайте новую базу данных:
```bash
isql
CREATE DATABASE '<path-to-db>\TaskDB.fdb' USER 'SYSDBA' PASSWORD '<your-password>' PAGE_SIZE 16384 DEFAULT CHARACTER SET UTF8;
```
3. Выполните SQL скрипт из файла `TaskManagement.API/Database/CreateDatabase.sql` для создания таблиц и начальных данных

### 2. Backend (API)

```bash
cd TaskManagement.API
dotnet restore
dotnet run
```

API будет доступен по адресу: http://localhost:5258/swagger

### 3. Frontend

```bash
cd task-management-frontend
npm install
npm run dev
```

Приложение будет доступно по адресу: http://localhost:5258

## Конфигурация

### Настройка подключения к БД

В файле `TaskManagement.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "FirebirdConnection": "User=SYSDBA;Password=<your-password>;Database=<path-to-db>\\TaskDB.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=UTF8;ServerType=0"
  }
}
```

## Архитектура

### Backend
- **Controllers** - REST API контроллеры
- **Services** - Бизнес-логика
- **Repositories** - Работа с БД через Dapper
- **DTOs** - Data Transfer Objects
- **Validators** - FluentValidation валидаторы
- **Models** - Доменные модели

### Frontend
- **components** - Vue компоненты
- **api** - Слой работы с API
- Использование Composition API

## API Endpoints

- `GET /api/tasks` - Получить все задачи
- `GET /api/tasks/{id}` - Получить задачу по ID
- `POST /api/tasks` - Создать задачу
- `PUT /api/tasks/{id}` - Обновить задачу
- `DELETE /api/tasks/{id}` - Удалить задачу
- `PATCH /api/tasks/{id}/status` - Изменить статус задачи
- `GET /api/tasks/by-status/{status}` - Получить задачи по статусу
- `GET /api/tasks/recent` - Получить задачи за последнюю неделю
- `GET /api/tasks/statistics` - Получить статистику

## Использованные компоненты

В этом проекте используется следующий стек:
- **Backend**: REST API на ASP.NET Core 8 (не Web Forms)
- **Frontend**: Vue.js 3 с vuedraggable для Drag&Drop (не DevExpress)
- **ORM**: Dapper для работы с БД
- **Валидация**: FluentValidation

## Docker (опционально)

Если используется Docker:

```dockerfile
# Backend
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]
```

## IIS развертывание

Для развертывания на IIS:
1. Установите .NET Core Hosting Bundle
2. Опубликуйте приложение: `dotnet publish -c Release`
3. Создайте новый сайт в IIS и укажите путь к опубликованному приложению


