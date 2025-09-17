using C_Part1;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class TaskNotificationListener
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "tasks_exchange";

    public TaskNotificationListener(string hostName = "localhost")
    {
        var factory = new ConnectionFactory() { HostName = hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);

        // Постоянная очередь для всех слушателей
        var queueName = _channel.QueueDeclare(queue: "tasks_notifications", durable: true, exclusive: false, autoDelete: false).QueueName;
        _channel.QueueBind(queue: queueName, exchange: ExchangeName, routingKey: "");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceived;
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        Console.WriteLine("Listener запущен. Ожидание событий...");
    }

    private void OnMessageReceived(object sender, BasicDeliverEventArgs e)
    {
        var body = e.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        try
        {
            var jsonDoc = JsonDocument.Parse(message);
            var root = jsonDoc.RootElement;

            string eventType = root.GetProperty("Event").GetString() ?? "Unknown";

            switch (eventType)
            {
                case "Create":
                    var createdTask = root.GetProperty("Task").Deserialize<TaskItem>();
                    Console.WriteLine($"[Created] Новая задача: {createdTask?.Id} - {createdTask?.Title}");
                    break;

                case "Update":
                    var updatedTaskId = root.GetProperty("TaskId").GetInt32();
                    Console.WriteLine($"[Updated] Задача обновлена: ID {updatedTaskId}");
                    break;

                case "Delete":
                    var deletedTaskId = root.GetProperty("TaskId").GetInt32();
                    Console.WriteLine($"[Deleted] Задача удалена: ID {deletedTaskId}");
                    break;

                default:
                    Console.WriteLine($"[Unknown event] {message}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке сообщения: {ex.Message}");
        }
    }
}
