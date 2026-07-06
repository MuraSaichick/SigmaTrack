# ⚙️ SigmaTrack — Документация Backend API

Бэкенд разработан на платформе **.NET 10** с использованием **ASP.NET Core Minimal APIs**. Архитектура построена на принципах **Clean Architecture** и разделения на вертикальные срезы (**Vertical Slices**).

---

## 🛠️ Технологический стек & Особенности

* **СУБД:** PostgreSQL с автоматическим преобразованием имен в *snake_case* (`UseSnakeCaseNamingConvention`).
* **ORM:** Entity Framework Core (реализован паттерн Repository + Unit of Work).
* **Валидация:** FluentValidation (с интеграцией в глобальную обработку ошибок).
* **Документация API:** Нативный `Microsoft.AspNetCore.OpenApi` + интерактивный UI **Scalar**.

---

## 📐 Архитектурные решения

### ⚡ Отказ от MediatR в пользу прямых UseCase
Вместо классического CQRS через MediatR, который часто усложняет навигацию по коду, в проекте используется **прямой вызов UseCase-компонентов**. Каждый срез (Feature) изолирует свою логику:
* Эндпоинт принимает запрос -> Маппит данные в Command/Query -> Вызывает `IUseCase.ExecuteAsync()`.

### 🧩 Модульные Эндпоинты (`IEndpointModule`)
Для предотвращения разрастания файла `Program.cs` используется интерфейс `IEndpointModule`. При старте приложения система автоматически сканирует сборку WebApi через Reflection, находит все модули эндпоинтов и регистрирует их маршруты:

### 💉 Автоматическое управление зависимостями (DI)

Для минимизации ручной работы по регистрации сервисов в DI-контейнере реализованы кастомные методы расширения, которые инкапсулируют логику внутри своих слоев:

* `builder.Services.AddApplicationUseCases(...)` — сканирует слой Application и регистрирует все UseCase-сервисы.
* `builder.Services.AddInfrastructureRepositories(...)` — сканирует слой Infrastructure и регистрирует репозитории.

---

## 🔒 Аутентификация, Авторизация и Scalar-хак

Безопасность обеспечивается на базе **JWT Bearer Authentication** и Claims-авторизации. Приватные эндпоинты защищены цепочкой `.RequireAuthorization()`.

### 🛠️ Интеграция со Scalar в Development-окружении

Для повышения удобства локального тестирования в `Program.cs` встроен кастомный Middleware. Если запрос поступает из интерфейса документации Scalar и в нем отсутствует заголовок `Authorization`, система **автоматически подставляет тестовый JWT-токен** из конфигурации (`Jwt:TestToken`). Это избавляет от необходимости постоянно копировать токен вручную при перезапуске API.

---

## 🚨 Глобальная обработка ошибок (`RFC 7807`)

В приложении задействован интерфейс `IExceptionHandler` реализованный в классе `GlobalExceptionHandler`. Любое необработанное исключение перехватывается и маппится в стандартизированный формат ответов **Problem Details (RFC 7807)**.

### Маппинг исключений в HTTP-статусы:

| Тип исключения | HTTP Статус | Описание |
| --- | --- | --- |
| `ValidationException` (FluentValidation) | **400 Bad Request** | Возвращает `HttpValidationProblemDetails` со списком ошибок, сгруппированных по полям. |
| `DomainException` | **400 Bad Request** | Нарушение бизнес-правил ядра системы. |
| `ArgumentException` | **400 Bad Request** | Некорректные входные параметры. |
| `KeyNotFoundException` | **404 Not Found** | Запрашиваемый ресурс не найден в БД. |
| `UnauthorizedAccessException` | **401 Unauthorized** | Отказ в доступе к ресурсу. |
| Любое другое исключение | **500 Internal Server Error** | Непредвиденная ошибка сервера (логируется через `ILogger`). |

---

## 🚀 Базовые команды CLI для разработки

Применение миграций и обновление локальной базы данных PostgreSQL (выполнять из папки решения):

```bash
dotnet ef database update --project SigmaTrack.Infrastructure --startup-project SigmaTrack.WebApi

```

Добавление новой миграции при изменении доменных моделей:

```bash
dotnet ef migrations add NameOfMigration --project SigmaTrack.Infrastructure --startup-project SigmaTrack.WebApi

```

## 🐳 Контейнеризация бэкенда (Docker)

Для сборки API-слоя используется оптимизированный многоэтапный Dockerfile (**Multi-stage build**), разделяющий тяжелое окружение сборки и легковесный рантайм для исполнения:

1. **Этап сборки (`build` & `publish`):** Базируется на образе `mcr.microsoft.com/dotnet/sdk:10.0`. На этом этапе сначала изолированно копируются файлы проектов (`.csproj`) и вызывается `dotnet restore`. Это позволяет кэшировать слои с NuGet-пакетами и не скачивать их заново, если код изменился, а зависимости остались прежними. Затем проект компилируется в режиме `Release`.
2. **Финальный этап (`final`):** Базируется на чистом рантайме `mcr.microsoft.com/dotnet/aspnet:10.0`. В него переносятся только скомпилированные DLL-библиотеки из этапа публикации, что гарантирует минимальный размер итогового контейнера и повышает безопасность.

### Сетевая конфигурация контейнера
Контейнер бэкенда жестко настроен на работу по порту `5069`:
```dockerfile
EXPOSE 5069
ENV ASPNETCORE_URLS=http://+:5069

```

## ⚙️ Конфигурация (`appsettings.json`)

Для локального запуска скопируйте `appsettings.json` в `appsettings.Development.json` и заполните параметры. Ниже представлен эталонный шаблон конфигурации:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SigmaTrackDb;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY_MINIMUM_32_CHARACTERS_LONG_12345!",
    "Issuer": "SigmaTrackAuthServer",
    "Audience": "SigmaTrackClientApp",
    "DurationInMinutes": 60,
    "RefreshTokenDurationInDays": 7,
    "TestToken": "Для_Автоматической_Авторизации_В_Scalar"
  }
}

```

> ⚠️ **Важно:** Заголовок `Jwt:Key` в Production-окружении должен содержать криптостойкий ключ длиной не менее 256 бит (32 символа). Значение `Jwt:TestToken` используется исключительно в `Development` режиме для автоматического проброса авторизации при отправке запросов через UI Scalar.