namespace MessageProcessingService;

public class ProcessingService
{
    private readonly IMessageQueueReceiver _messageQueueReceiver;
    private readonly IMongoRepository _mongoRepository;
    private readonly NotificationManager _notificationManager;

    public ProcessingService(
        IMessageQueueReceiver messageQueueReceiver,
        IMongoRepository mongoRepository,
        NotificationManager notificationManager)
    {
        _messageQueueReceiver = messageQueueReceiver;
        _mongoRepository = mongoRepository;
        _notificationManager = notificationManager;
    }

    public void Start()
    {
        _notificationManager.Start();
        _messageQueueReceiver.StartReceiving(HandleServerStatistics);
    }

    private void HandleServerStatistics(StatisticsReceived statistics)
    {
        var previousStatistics = _mongoRepository.GetLastRecord(); 
        _notificationManager.CheckAlerts(statistics, previousStatistics); 

        Console.WriteLine(statistics);

        _mongoRepository.Insert(statistics);
    }
}