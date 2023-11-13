namespace MessageProcessingService;

public enum Alert : byte
{
    None,
    MemoryAnomaly,
    CpuAnomaly,
    MemoryHighUsage,
    CpuHighUsage
}