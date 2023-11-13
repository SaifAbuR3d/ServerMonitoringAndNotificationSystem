using MongoDB.Bson.Serialization.Attributes;

namespace MessageProcessingService;

[BsonIgnoreExtraElements]
public class StatisticsReceived
{
    public string ServerIdentifier { get; set; }
    public double MemoryUsage { get; set; }
    public double AvailableMemory { get; set; }
    public double CpuUsagePercentage { get; set; }
    public DateTime Timestamp { get; set; }
    public double MemoryUsagePercentage => MemoryUsage / (MemoryUsage + AvailableMemory) * 100;

    public override string ToString()
    {
        return $"{{Memory Usage={Math.Round(MemoryUsage, 2)}MB, Available Memory={AvailableMemory}MB, Memory Usage Percentage={Math.Round(MemoryUsagePercentage, 2)}%, Cpu Usage Percentage={Math.Round(CpuUsagePercentage, 2)}%, Time={Timestamp}}}";
    }
}