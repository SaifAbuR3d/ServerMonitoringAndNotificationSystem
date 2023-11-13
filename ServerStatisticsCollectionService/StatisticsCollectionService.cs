using System.Diagnostics;
namespace ServerStatisticsCollectionService;

public class StatisticsCollectionService
{
    private readonly string _serverIdentifier;
    private readonly int _samplingIntervalSeconds;
    private readonly IMessageQueueSender _messageQueue;
    private readonly PerformanceCounter _cpuCounter; 
    private readonly PerformanceCounter _memoryCounter;

    public StatisticsCollectionService(string serverIdentifier, int samplingIntervalSeconds, IMessageQueueSender messageQueue)
    {
        _serverIdentifier = serverIdentifier;
        _samplingIntervalSeconds = samplingIntervalSeconds;
        _messageQueue = messageQueue;
        _cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
        _memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

        // ignore first value as it will always start at 0
        _cpuCounter.NextValue();
    }

    public void Start()
    {
        System.Timers.Timer timer = new(interval: _samplingIntervalSeconds * 1000);
        timer.Elapsed += async (sender, e) => await CollectAndPublishServerStatistics(null);
        timer.Start();
    }

    private Task CollectAndPublishServerStatistics(object state)
    {
        double memoryUsage = GetMemoryUsage();
        double availableMemory = GetAvailableMemory();
        double cpuUsage = GetCpuUsage();

        var statistics = new StatisticsSent
        {
            MemoryUsage = memoryUsage,
            AvailableMemory = availableMemory,
            CpuUsagePercentage = cpuUsage,
            Timestamp = DateTime.Now
        };

        _messageQueue.Publish($"ServerStatistics.{_serverIdentifier}", statistics);
        return Task.CompletedTask;
    }
    private double GetMemoryUsage()
    {
        return GetTotalMemory() - GetAvailableMemory();
    }

    private double GetTotalMemory()
    {
        var gcMemoryInfo = GC.GetGCMemoryInfo();
        var installedMemory = gcMemoryInfo.TotalAvailableMemoryBytes/(1024.0*1024.0);
        return installedMemory;
    }

    private double GetAvailableMemory()
    {
        return _memoryCounter.NextValue();
    }

    public double GetCpuUsage()
    {
        return _cpuCounter.NextValue();
    }
}