namespace Libellus.Application.Common.Interfaces.Measuring;

public interface ITimer
{
    bool IsRunning { get; }
    TimeSpan Elapsed { get; }

    void Start();

    void Stop();

    void Reset();

    static abstract TimeSpan Time(Action action);
}