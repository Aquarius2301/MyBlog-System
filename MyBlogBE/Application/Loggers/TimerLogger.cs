namespace Application.Loggers;

/// <summary>
/// A logger specifically for timing operations, logging start and end times along with durations.
/// </summary>
public class TimerLogger : MyBlogLogger
{
    private DateTime _timeStarted;
    private string _traceId = "";

    public TimerLogger()
        : base("timer.log") { }

    /// <summary>
    /// Logs the start of a timed operation.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task LogStart(object? message = null)
    {
        _timeStarted = DateTime.Now;
        _traceId = Guid.NewGuid().ToString();
        var logEntry = $"[{_timeStarted:yyyy-MM-dd HH:mm:ss}] START\n";
        logEntry += $"Trace ID: {_traceId}\n";

        if (message != null)
            logEntry += $"Message: {message}\n";

        await File.AppendAllTextAsync(_logPath, logEntry + "\n");
    }

    /// <summary>
    /// Logs the end of a timed operation, including duration.
    /// </summary>
    /// <param name="message">An optional message to include in the log</param>
    /// <throws>InvalidOperationException if LogStart was not called before LogEnd</throws>
    public async Task LogEnd(object? message = null)
    {
        if (_timeStarted == default || string.IsNullOrEmpty(_traceId))
            throw new InvalidOperationException("LogStart must be called before LogEnd.");

        var timeEnded = DateTime.Now;
        var duration = timeEnded - _timeStarted;

        var logEntry = $"[{timeEnded:yyyy-MM-dd HH:mm:ss}] END\n";
        logEntry += $"Trace ID: {_traceId}\n";
        logEntry += $"Duration: {duration.TotalSeconds} s\n";

        if (message != null)
            logEntry += $"Message: {message}\n";

        await File.AppendAllTextAsync(_logPath, logEntry + "\n");
    }
}
