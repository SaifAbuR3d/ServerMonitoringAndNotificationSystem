namespace ServerStatisticsCollectionService;

public class StatisticsSent
{
    public double MemoryUsage { get; set; }
    public double AvailableMemory { get; set; }
    public double CpuUsagePercentage { get; set; }
    public DateTime Timestamp { get; set; }

    public override string ToString()
    {
        return $"{{Memory Usage={Math.Round(MemoryUsage, 2)}MB, Available Memory={AvailableMemory}MB, Cpu Usage={Math.Round(CpuUsagePercentage, 2)}%, Time={Timestamp}}}";
    }
}