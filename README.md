# Winton-Notification
Выбранное задание: 
 - "Разработка распределенного сервиса отправки уведомлений на разные источники"
Команда Winton:
- Бондаренко Дмитрий Андреевич TeamLead
- Романов Евгений Сергеевич

## Быстрый старт

### Запуск инфраструктуры

```bash
docker-compose up -d
```

### Запуск Gateway

из корня проекта
```bash
dotnet run --project notification-service/Gateway
```

Gateway будет доступен на:
- **HTTP:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger

### Тестирование

Подробная инструкция по тестированию находится в [TESTING.md](notification-service/Gateway/TESTING.md)

### Подключение к сервисам

**Краткая справка:**

- **RabbitMQ Management UI:** http://localhost:15672 (admin/admin123)
- **PostgreSQL:** localhost:5432 (postgres/postgres123, БД: notificationdb)
- **Gateway API:** http://localhost:5000/swagger
