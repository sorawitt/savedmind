namespace SavedMind.Application.Common.Exceptions;

/// <summary>
/// Base exception for all application-level errors.
/// These are errors that occur during use case execution (e.g., validation against database).
/// </summary>
public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message) : base(message) { }

    protected ApplicationException(string message, Exception innerException)
        : base(message, innerException) { }
}
