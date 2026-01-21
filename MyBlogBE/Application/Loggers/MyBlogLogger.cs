namespace Application.Loggers;

/// <summary>
/// A simple logger for logging informational, warning, and error messages to a specified log file.
/// </summary>
public class MyBlogLogger
{
    protected string _logPath = string.Empty;

    public MyBlogLogger(string logPath = "common_log.log")
    {
        Directory.CreateDirectory("Logs");
        _logPath = "Logs/" + logPath;
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="taskName"> The name of the task being logged. </param>
    /// <param name="message"> Additional information to log. </param>
    public async Task LogInfo(string taskName, object? message = null)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n";
        logEntry += $"Task Name: {taskName}\n";
        if (message != null)
            logEntry += $"Info: {message}\n";

        await File.AppendAllTextAsync(_logPath, logEntry + "\n");
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="taskName"> The name of the task being logged. </param>
    /// <param name="message"> Additional warning information to log. </param>
    public async Task LogWarning(string taskName, object? message = null)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n";
        logEntry += $"Task Name: {taskName}\n";
        if (message != null)
            logEntry += $"Warning: {message}\n";

        await File.AppendAllTextAsync(_logPath, logEntry + "\n");
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="taskName"> The name of the task being logged. </param>
    /// <param name="error"> Additional error information to log. </param>
    public async Task LogError(string taskName, object? error = null)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n";
        logEntry += $"Task Name: {taskName}\n";
        if (error != null)
            logEntry += $"Error: {error}\n";

        await File.AppendAllTextAsync(_logPath, logEntry + "\n");
    }
}
