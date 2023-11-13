using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ServerStatisticsCollectionService;

public class RabbitMQSender : IMessageQueueSender, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQSender(string hostName, int port, string userName, string password)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password
        };
        factory.ClientProvidedName = "Server StatisticsSent Collector Service";

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

    }

    public void Publish(string topic, StatisticsSent message)
    {
        string exchangeName = "Topic Exchange";
        string queueName = $"Topic Queue";

        _channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
        _channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false);
        _channel.QueueBind(queueName, exchangeName, topic);


        string jsonMessage = JsonSerializer.Serialize(message);
        var bytesMessage = Encoding.UTF8.GetBytes(jsonMessage);

        _channel.BasicPublish(
            exchange: exchangeName,
            routingKey: topic,
            basicProperties: null,
            body: bytesMessage
        );


    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}