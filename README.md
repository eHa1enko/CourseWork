# Music Platform
## Вимоги

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

Більше нічого встановлювати не потрібно — .NET SDK і Node.js запускаються всередині контейнерів.

## Швидкий старт

### 1. Налаштуйте Cloudinary

Відкрийте `docker-compose.yml` і замініть значення в секції `api → environment`:

```yaml
Cloudinary__CloudName: "ваш_cloud_name"
Cloudinary__ApiKey: "ваш_api_key"
Cloudinary__ApiSecret: "ваш_api_secret"
```

Отримати ключі можна безкоштовно на [cloudinary.com](https://cloudinary.com).

### 2. Запустіть весь проєкт

```bash
docker compose up
```

Це автоматично:
- підніме SQL Server з named volume (дані зберігаються між перезапусками)
- дочекається готовності БД через healthcheck
- виконає EF Core міграції при старті API
- створить адміна, якщо його ще немає
- запустить Angular dev-сервер

### 3. Відкрийте застосунок

| Сервіс   | URL                   |
|----------|-----------------------|
| Frontend | http://localhost:4200 |
| API      | http://localhost:5094 |

## Облікові дані адміна

За замовчуванням (налаштовуються через `AdminSettings` у `docker-compose.yml`):

- **Email:** `admin@example.com`
- **Password:** `Admin@1234!`

## Зупинка

```bash
# зупинити контейнери (дані БД зберігаються)
docker compose down

# зупинити і видалити дані БД
docker compose down -v
```

## Локальна розробка без Docker

Потрібні: .NET 10 SDK, Node.js, SQL Server.

1. Вкажіть ваш ConnectionString і Cloudinary-ключі в `CourseWork.API/appsettings.Development.json`

2. Запустіть API (міграції і адмін створяться автоматично):
   ```bash
   cd CourseWork.API && dotnet run
   ```

3. Запустіть фронтенд:
   ```bash
   cd CourseWork.UI && npm install && ng serve
   ```
