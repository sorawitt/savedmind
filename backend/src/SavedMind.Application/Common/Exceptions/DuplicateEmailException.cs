namespace SavedMind.Application.Common.Exceptions;

/// <summary>
/// Thrown when attempting to register with an email that already exists in the database.
/// This is an Application concern because it requires querying external data.
/// </summary>
public class DuplicateEmailException : ApplicationException
{
    public string Email { get; }

    public DuplicateEmailException(string email)
        : base($"A user with email '{email}' already exists.")
    {
        Email = email;
    }
}
