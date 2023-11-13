namespace MessageProcessingService;

public class NotificationManager
{
    private readonly double _memoryUsageAnomalyThresholdPercentage;
    private readonly double _cpuUsageAnomalyThresholdPercentage;
    private readonly double _memoryUsageThresholdPercentage;
    private readonly double _cpuUsageThresholdPercentage;
    private readonly SignalRClient _signalRClient;

    public NotificationManager(double memoryUsageAnomalyThresholdPercentage,
                               double cpuUsageAnomalyThresholdPercentage,
                               double memoryUsageThresholdPercentage,
                               double cpuUsageThresholdPercentage,
                               SignalRClient signalRClient)
    {
        _memoryUsageAnomalyThresholdPercentage = memoryUsageAnomalyThresholdPercentage;
        _cpuUsageAnomalyThresholdPercentage = cpuUsageAnomalyThresholdPercentage;
        _memoryUsageThresholdPercentage = memoryUsageThresholdPercentage;
        _cpuUsageThresholdPercentage = cpuUsageThresholdPercentage;
        _signalRClient = signalRClient;
    }

    public void Start()
    {
        _signalRClient.Start();
    }

    public void CheckAlerts(StatisticsReceived statistics, StatisticsReceived? previousStatistics)
    {
        CheckMemoryUsagePercentage(statistics.MemoryUsagePercentage, _memoryUsageThresholdPercentage);
        CheckCpuUsagePercentage(statistics.CpuUsagePercentage, _cpuUsageThresholdPercentage);

        if (previousStatistics == null)
        {
            return;
        }
        CheckMemoryUsageAnomaly(statistics.MemoryUsage, previousStatistics.MemoryUsage, _memoryUsageAnomalyThresholdPercentage);
        CheckCpuUsageAnomaly(statistics.CpuUsagePercentage, previousStatistics.CpuUsagePercentage, _cpuUsageAnomalyThresholdPercentage);
    }

    private void CheckCpuUsageAnomaly(double currentCpuUsagePercentage1, double previousCpuUsagePercentage, double cpuUsageAnomalyThresholdPercentage)
    {
        if (currentCpuUsagePercentage1 > previousCpuUsagePercentage * (1 + cpuUsageAnomalyThresholdPercentage))
        {
            _signalRClient.SendAlert(Alert.CpuAnomaly);
        }
    }

    private void CheckMemoryUsageAnomaly(double currentMemoryUsage, double previousMemoryUsage, double memoryUsageThresholdPercentage)
    {
        if (currentMemoryUsage > previousMemoryUsage * (1 + memoryUsageThresholdPercentage))
        {
            _signalRClient.SendAlert(Alert.MemoryAnomaly);
        }
    }

    private void CheckCpuUsagePercentage(double cpuUsagePercentage, double cpuUsageThresholdPercentage)
    {
        if (cpuUsagePercentage > cpuUsageThresholdPercentage * 100)
        {
            _signalRClient.SendAlert(Alert.CpuHighUsage);
        }
    }

    private void CheckMemoryUsagePercentage(double memoryUsagePercentage, double memoryUsageThresholdPercentage)
    {
        if (memoryUsagePercentage > memoryUsageThresholdPercentage * 100)
        {
            _signalRClient.SendAlert(Alert.MemoryHighUsage);
        }
    }
}
