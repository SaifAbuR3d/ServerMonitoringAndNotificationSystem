using MongoDB.Driver;

namespace MessageProcessingService;

public class MongoDBRepository : IMongoRepository
{
    private readonly IMongoCollection<StatisticsReceived> _collection;

    public MongoDBRepository(string connectionString)
    {
        string databaseName = "MessageProcessingService";
        string collectionName = "ServerStatisticsInfo";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<StatisticsReceived>(collectionName);
    }

    public void Insert(StatisticsReceived statistics)
    {
        _collection.InsertOne(statistics);
    }

    public StatisticsReceived? GetLastRecord()
    {
        return _collection.Find(_ => true).SortByDescending(e => e.Timestamp).FirstOrDefault();
    }
}