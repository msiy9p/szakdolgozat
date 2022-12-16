using Libellus.Application.Common.Interfaces.Measuring;
using System.Diagnostics;

namespace Libellus.Application.Utilities.Measuring;

public sealed class CpuTimer : ITimer
{
    private TimeSpan _startTime;
    private TimeSpan _endTime;

    public bool IsRunning { get; private set; }

    public TimeSpan Elapsed
    {
        get
        {
            if (IsRunning)
            {
                return TimeSpan.Zero;
            }

            return _endTime - _startTime;
        }
    }

    public void Start()
    {
        _startTime = Process.GetCurrentProcess().TotalProcessorTime;
        IsRunning = true;
    }

    public void Stop()
    {
        _endTime = Process.GetCurrentProcess().TotalProcessorTime;
        IsRunning = false;
    }

    public void Reset()
    {
        _startTime = TimeSpan.Zero;
        _endTime = TimeSpan.Zero;
    }

    public static CpuTimer StartNew()
    {
        var timer = new CpuTimer();
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