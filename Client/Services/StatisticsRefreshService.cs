using System;

namespace Client.Services;

public class StatisticsRefreshService
{
    public event Action? OnStatisticsChanged;

    public void NotifyStatisticsChanged()
    {
        OnStatisticsChanged?.Invoke();
    }
}
