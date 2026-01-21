namespace Application.Loggers;

public class ApiLogger : MyBlogLogger
{
    private DateTime _timeStarted;
    private string _traceId = "";

    public ApiLogger()
        : base("api.log") { }

    /// <summary>
    /// Log API request details
    /// </summary>
    /// <param name="path">The path of the API request</param>
    /// <param name="contentType">The content type of the request</param>
    /// <param name="body">The body of the request</param>
    public async Task LogRequest(
        string method,
        string path,
        string language,
        object? contentType = null,
        object? body = null
    )
    {
        _timeStarted = DateTime.Now;
        _traceId = Guid.NewGuid().ToString();

        var logEntry = $"[{_timeStarted:yyyy-MM-dd HH:mm:ss}] REQUEST\n";
        logEntry += $"Trace ID: {_traceId}\n";
        logEntry += $"Method: {method}\n";
        logEntry += $"Path: {path}\n";
        logEntry += $"Language: {language}\n";

        if (contentType != null)
            logEntry += $"ContentType: {contentType}\n";

        if (body != null)
            logEntry += $"Body: {body}\n";

        await File.AppendAllTextAsync(_logPath, logEntry + "\n");
    }

    /// <summary>
    /// Log API response details
    /// </summary>
    /// <param name="statusCode">The status code of the response</param>
    /// <param name="body">The body of the response</param>
    /// <throws>InvalidOperationException if LogRequest was not called before LogResponse</throws>
    public async Task LogResponse(int statusCode, object? body = null)
    {
        if (_timeStarted == default || string.IsNullOrEmpty(_traceId))
            throw new InvalidOperationException("LogRequest must be called before LogResponse.");

        var timeEnded = DateTime.Now;
        var duration = timeEnded - _timeStarted;

        var logEntry = $"[{timeEnded:yyyy-MM-dd HH:mm:ss}] RESPONSE\n";
        logEntry += $"Trace ID: {_traceId}\n";
        logEntry += $"StatusCode: {statusCode}\n";
        logEntry += $"Duration: {duration.TotalSeconds} s\n";

        if (body != null)
            logEntry += $"Body: {body}\n";

        await File.AppendAllTextAsync(_logPath, logEntry + "\n");
    }
}
