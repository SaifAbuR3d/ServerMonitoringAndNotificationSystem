namespace MessageProcessingService;

public interface IMongoRepository
{
    StatisticsReceived? GetLastRecord();
    void Insert(StatisticsReceived statistics);
}