using Microsoft.Extensions.Configuration;

namespace ServerStatisticsCollectionService;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var messageQueueSender = GetMessageQueueSender(configuration);

        var statisticsCollectionService = GetStatisticsCollectionService(configuration, messageQueueSender);

        statisticsCollectionService.Start();
        Console.ReadKey();

    }
    private static RabbitMQSender GetMessageQueueSender(IConfigurationRoot configuration)
    {
        var hostName = configuration.GetSection("RabbitMQConfig")["HostName"];
        var port = Convert.ToInt32(configuration.GetSection("RabbitMQConfig")["Port"]);
        var userName = configuration.GetSection("RabbitMQConfig")["UserName"];
        var password = configuration.GetSection("RabbitMQConfig")["Password"];

        var messageQueue = new RabbitMQSender(hostName, port, userName, password);
        return messageQueue;
    }

    private static StatisticsCollectionService GetStatisticsCollectionService(IConfigurationRoot configuration, RabbitMQSender messageQueue)
    {
        var samplingIntervalSeconds = Convert.ToInt32(configuration.GetSection("ServerStatisticsConfig")["SamplingIntervalSeconds"]);
        var serverIdentifier = configuration.GetSection("ServerStatisticsConfig")["ServerIdentifier"];

        var statisticsCollectionService = new StatisticsCollectionService(serverIdentifier, samplingIntervalSeconds, messageQueue);
        return statisticsCollectionService;
    }

}