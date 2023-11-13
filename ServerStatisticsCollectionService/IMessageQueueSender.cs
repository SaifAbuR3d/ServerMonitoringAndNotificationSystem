namespace ServerStatisticsCollectionService;

public interface IMessageQueueSender
{
    void Publish(string topic, StatisticsSent message);
}