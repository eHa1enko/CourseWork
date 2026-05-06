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

Ключі додані в приватному коментарі гугл класу.

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

Потрібні: [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), [Node.js](https://nodejs.org/en/download), [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).

1. Відкрийте `CourseWork.API/appsettings.Development.json` і вкажіть свій рядок підключення до SQL Server.

   Якщо встановили SQL Server Express з налаштуваннями за замовчуванням — рядок вже правильний:
   ```
   Server=localhost\SQLEXPRESS;Database=MusicPlatform;Trusted_Connection=True;TrustServerCertificate=True;
   ```
   База даних `MusicPlatform` **створюється автоматично** при першому запуску — вручну створювати не потрібно.

2. Вкажіть Cloudinary-ключі в тому ж файлі:
   ```json
   "Cloudinary": {
     "CloudName": "ваш_cloud_name",
     "ApiKey": "ваш_api_key",
     "ApiSecret": "ваш_api_secret"
   }
   ```

3. Запустіть API (міграції, БД і адмін створяться автоматично):
   ```bash
   cd CourseWork.API && dotnet run
   ```

4. Запустіть фронтенд:
   ```bash
   cd CourseWork.UI && npm install && ng serve
   ```
