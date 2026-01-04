namespace SavedMind.Application.Common.Exceptions;

public class InvalidCredentialsException : ApplicationException
{
    public InvalidCredentialsException()
        : base("Invalid email or password.")
    { }
}