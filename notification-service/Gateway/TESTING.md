# Тестирование Notification Gateway

## Шаг 1: Запуск инфраструктуры

Откройте терминал в корне проекта и выполните:

```bash
docker-compose up -d
```

Проверьте, что контейнеры запущены:

```bash
docker ps --filter "name=winton"
```

Должны быть запущены:
- `winton-rabbitmq`
- `winton-postgres`

## Шаг 2: Запуск Gateway

```bash
dotnet run --project notification-service/Gateway
```

Gateway запустится на:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

Swagger UI будет доступен по адресу: http://localhost:5000/swagger

## Шаг 3: Тестирование через Swagger UI

1. Откройте браузер и перейдите на: http://localhost:5000/swagger
2. Найдите эндпоинт `POST /api/notifications`
3. Нажмите "Try it out"
4. Вставьте JSON в поле "Request body":

```json
{
  "type": 1,
  "recipient": "test@example.com",
  "subject": "Test Notification",
  "message": "Это тестовое уведомление"
}
```

Где `type`:
- `1` = Email
- `2` = SMS
- `3` = Push

5. Нажмите "Execute"
6. Должен вернуться ответ `202 Accepted` с `status: "queued"`

## Шаг 4: Тестирование через curl (PowerShell)

# Email уведомление:
```powershell
curl -X POST http://localhost:5000/api/notifications `
  -H "Content-Type: application/json" `
  -d '{\"type\":1,\"recipient\":\"test@example.com\",\"subject\":\"Test Email\",\"message\":\"Hello from Gateway\"}'
```

# SMS уведомление:
```powershell
curl -X POST http://localhost:5000/api/notifications `
  -H "Content-Type: application/json" `
  -d '{\"type\":2,\"recipient\":\"+79991234567\",\"message\":\"Test SMS message\"}'
```

# Push уведомление:
```powershell
curl -X POST http://localhost:5000/api/notifications `
  -H "Content-Type: application/json" `
  -d '{\"type\":3,\"recipient\":\"device-token-123\",\"message\":\"Test Push notification\"}'
```

## Шаг 5: Проверка в RabbitMQ Management UI

1. Откройте браузер: http://localhost:15672
2. Войдите:
   - Username: `admin`
   - Password: `admin123`
3. Перейдите в раздел "Queues"
4. Вы должны увидеть очереди:
   - `notifications.email`
   - `notifications.sms`
   - `notifications.push`
5. Выберите очередь (например, `notifications.email`)
6. В разделе "Get messages" нажмите "Get message(s)"
7. Вы увидите сообщение в формате JSON с вашим уведомлением

## Шаг 6: Проверка логов Gateway

В терминале, где запущен Gateway, вы должны увидеть логи:

```
info: Gateway.Services.RabbitMQService[0]
      RabbitMQ connection established
info: Gateway.Controllers.NotificationsController[0]
      Notification accepted and queued. Id: ..., Type: Email, Recipient: test@example.com
info: Gateway.Services.RabbitMQService[0]
      Notification published to queue notifications.email, Id: ...
```

## Примеры тестовых запросов

# Полный запрос с всеми полями:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "type": 1,
  "recipient": "user@example.com",
  "subject": "Важное уведомление",
  "message": "Это тестовое сообщение",
  "metadata": "{\"priority\":\"high\",\"category\":\"system\"}",
  "createdAt": "2024-12-06T12:00:00Z"
}
```

# Минимальный запрос (обязательные поля):
```json
{
  "type": 1,
  "recipient": "user@example.com",
  "message": "Простое сообщение"
}
```

## Остановка сервисов

Для остановки Gateway: нажмите `Ctrl+C` в терминале

Для остановки инфраструктуры:
```bash
docker-compose down
```

Для полной очистки (включая данные):
```bash
docker-compose down -v
```

