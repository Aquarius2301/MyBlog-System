using System;

namespace Application.Exceptions;

public class BaseException : Exception
{
    public int StatusCode { get; }
    public new object? Data { get; } = default;

    public BaseException(string message, int statusCode, object? data = null)
        : base(message)
    {
        StatusCode = statusCode;
        Data = data;
    }
}

public class BadRequestException : BaseException
{
    public BadRequestException(string message, object? data = null)
        : base(message, 400, data) { }
}

public class NotFoundException : BaseException
{
    public NotFoundException(string message, object? data = null)
        : base(message, 404, data) { }
}

public class ForbiddenException : BaseException
{
    public ForbiddenException(string message, object? data = null)
        : base(message, 403, data) { }
}

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message, object? data = null)
        : base(message, 401, data) { }
}
