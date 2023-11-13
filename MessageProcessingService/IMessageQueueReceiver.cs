namespace MessageProcessingService;

public interface IMessageQueueReceiver
{
    void StartReceiving(Action<StatisticsReceived> handleServerStatistics);
}