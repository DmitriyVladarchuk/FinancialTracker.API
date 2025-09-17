# Financial Tracker API

Простое RESTful API для управления личными финансами, написанное на ASP.NET Core.

## 🚀 Возможности

- **Управление категориями** - Полный CRUD для категорий доходов/расходов
- **Управление транзакциями** - Создание, чтение, обновление и удаление финансовых операций
- **Валидация данных** - Проверка входных данных и обработка ошибок
- **RESTful архитектура** - Соответствие стандартам REST API

## 🛠️ Стек

- **ASP.NET Core 9.0** - Веб-фреймворк
- **Entity Framework Core 9.0** - ORM для работы с базой данных
- **PostgreSQL** - Реляционная база данных
- **Npgsql** - PostgreSQL провайдер для EF Core
- **xUnit** - Фреймворк для unit-тестирования
- **Moq** - Mocking библиотека для тестов
- **GitHub Actions** - CI пайплайн

## 📚 API Endpoints

### Категории
- `GET /api/categories` - Получить все категории
- `GET /api/categories/{id}` - Получить категорию по ID
- `POST /api/categories` - Создать новую категорию
- `PUT /api/categories` - Обновить категорию
- `DELETE /api/categories/{id}` - Удалить категорию

### Транзакции
- `GET /api/transactions` - Получить все транзакции
- `GET /api/transactions/{id}` - Получить транзакцию по ID
- `POST /api/transactions` - Создать новую транзакцию
- `PUT /api/transactions` - Обновить транзакцию
- `DELETE /api/transactions/{id}` - Удалить транзакцию
