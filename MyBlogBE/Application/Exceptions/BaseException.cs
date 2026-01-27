using System;

namespace Application.Exceptions;

/// <summary>
/// Base exception class for application-specific exceptions.
/// </summary>
public class BaseException : Exception
{
    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets additional data related to the exception.
    /// </summary>
    public new object? Data { get; } = default;

    public BaseException(string message, int statusCode, object? data = null)
        : base(message)
    {
        StatusCode = statusCode;
        Data = data;
    }
}

/// <summary>
/// Represents a bad request exception (HTTP 400).
/// </summary>
public class BadRequestException : BaseException
{
    public BadRequestException(string message, object? data = null)
        : base(message, 400, data) { }
}

/// <summary>
/// Represents a not found exception (HTTP 404).
/// </summary>
public class NotFoundException : BaseException
{
    public NotFoundException(string message, object? data = null)
        : base(message, 404, data) { }
}

/// <summary>
/// Represents a forbidden exception (HTTP 403).
/// </summary>
public class ForbiddenException : BaseException
{
    public ForbiddenException(string message, object? data = null)
        : base(message, 403, data) { }
}

/// <summary>
/// Represents an unauthorized exception (HTTP 401).
/// </summary>
public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message, object? data = null)
        : base(message, 401, data) { }
}
