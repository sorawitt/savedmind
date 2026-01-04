namespace SavedMind.Application.Common.Exceptions;

public class EmailNotVerifiedException : ApplicationException
{
    public string Email { get; }

    public EmailNotVerifiedException(string email)
        : base($"Email {email} is not verified. Please check your inbox.")
    {
        Email = email;
    }
}