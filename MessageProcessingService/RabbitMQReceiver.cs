using MongoDB.Driver.Core.Bindings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace MessageProcessingService;

public class RabbitMQReceiver : IMessageQueueReceiver
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQReceiver(string hostName, int port, string userName, string password)
    {
        var factory = new ConnectionFactory
        {
            Port = port,
            HostName = hostName,
            UserName = userName,
            Password = password,
            ClientProvidedName = "Message Processing Service"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.BasicQos(0, 1, false);
    }

    public void StartReceiving(Action<StatisticsReceived> messageHandler)
    {
        string exchangeName = "Topic Exchange";
        string queueName = "Topic Queue";
        string routingKeyPrefix = "ServerStatistics.";

        _channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
        _channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false);
        _channel.QueueBind(queueName, exchangeName, routingKey: $"{routingKeyPrefix}*");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var statistics = JsonSerializer.Deserialize<StatisticsReceived>(message);

            statistics.ServerIdentifier = args.RoutingKey.Substring(routingKeyPrefix.Length);
            messageHandler(statistics);

            _channel.BasicAck(args.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queueName, autoAck: false, consumer);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}