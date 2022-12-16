using Libellus.Application.Common.Interfaces.Measuring;
using System.Diagnostics;

namespace Libellus.Application.Utilities.Measuring;

public sealed class StopwatchTimer : ITimer
{
    private readonly Stopwatch _stopwatch = new();

    public bool IsRunning => _stopwatch.IsRunning;
    public TimeSpan Elapsed => _stopwatch.Elapsed;

    public void Start() => _stopwatch.Start();

    public void Stop() => _stopwatch.Stop();

    public void Reset() => _stopwatch.Reset();

    public static StopwatchTimer StartNew()
    {
        var timer = new StopwatchTimer();
        timer.Start();
        return timer;
    }

    public static TimeSpan Time(Action action)
    {
        var timer = StartNew();

        action();

        timer.Stop();
        return timer.Elapsed;
    }
}