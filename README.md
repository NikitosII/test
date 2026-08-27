## Запуск

**1. База данных**

```bash
docker compose up -d
```

**2. Backend**

```bash
dotnet run --project src/Orders.Api
```

- API: <http://localhost:5134/api/orders>
- Swagger: <http://localhost:5134/swagger>

**3. Frontend**

```bash
cd client
npm install
npm run dev
```

- Приложение: <http://localhost:5173>


