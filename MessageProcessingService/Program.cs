using Microsoft.Extensions.Configuration;

namespace MessageProcessingService;

public class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var messageQueueReceiver = GetMessageQueueReceiver(configuration);

        var mongoRepository = GetMongoDbRepository(configuration);
        
        var notificationManager = GetNotificationManager(configuration); 
        
        var processingService = new ProcessingService(messageQueueReceiver, mongoRepository, notificationManager);

        
        processingService.Start();
        Console.ReadKey();

    }

    private static NotificationManager GetNotificationManager(IConfigurationRoot configuration)
    {
        var memoryUsageAnomalyThresholdPercentage = Convert.ToDouble(configuration.GetSection("NotificationManagerConfig")["MemoryUsageAnomalyThresholdPercentage"]);
        var cpuUsageAnomalyThresholdPercentage = Convert.ToDouble(configuration.GetSection("NotificationManagerConfig")["CpuUsageAnomalyThresholdPercentage"]);
        var memoryUsageThresholdPercentage = Convert.ToDouble(configuration.GetSection("NotificationManagerConfig")["MemoryUsageThresholdPercentage"]);
        var cpuUsageThresholdPercentage = Convert.ToDouble(configuration.GetSection("NotificationManagerConfig")["CpuUsageThresholdPercentage"]);

        var signalRClient = GetSignalRClient(configuration);

        var notificationManager = new NotificationManager(memoryUsageAnomalyThresholdPercentage,
                                                           cpuUsageAnomalyThresholdPercentage,
                                                           memoryUsageThresholdPercentage,
                                                           cpuUsageThresholdPercentage, 
                                                           signalRClient); 
        return notificationManager;
    }

    private static SignalRClient GetSignalRClient(IConfigurationRoot configuration)
    {
        string signalRHubUrl = configuration.GetConnectionString("SignalRHubUrl");
        var signalRClient = new SignalRClient(signalRHubUrl);
        return signalRClient;
    }

    private static MongoDBRepository GetMongoDbRepository(IConfigurationRoot configuration)
    {
        string mongoDbConnectionString = configuration.GetConnectionString("MongoDB");
        var mongoRepository = new MongoDBRepository(mongoDbConnectionString);
        return mongoRepository;
    }

    private static RabbitMQReceiver GetMessageQueueReceiver(IConfigurationRoot configuration)
    {
        var hostName = configuration.GetSection("RabbitMQConfig")["HostName"];
        var port = Convert.ToInt32(configuration.GetSection("RabbitMQConfig")["Port"]);
        var userName = configuration.GetSection("RabbitMQConfig")["UserName"];
        var password = configuration.GetSection("RabbitMQConfig")["Password"];

        var messageQueue = new RabbitMQReceiver(hostName, port, userName, password);
        return messageQueue;
    }
}