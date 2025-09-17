using C_Part1;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TaskServiceApi.Messaging
{
    public class RabbitMqPublisher
    {
        private const string ExchangeName = "tasks_exchange";
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
        }

        public void PublishTaskAdded(TaskItem task)
        {
            var json = JsonSerializer.Serialize(new { Event = "Create", Task = task });
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(exchange: ExchangeName, routingKey: "", basicProperties: null, body: body);
        }

        public void PublishTaskUpdated(int id)
        {
            var json = JsonSerializer.Serialize(new { Event = "Update", TaskId = id });
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(exchange: ExchangeName, routingKey: "", basicProperties: null, body: body);
        }

        public void PublishTaskDeleted(int id)
        {
            var json = JsonSerializer.Serialize(new { Event = "Delete", TaskId = id });
            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(exchange: ExchangeName, routingKey: "", basicProperties: null, body: body);
        }
    }
}
