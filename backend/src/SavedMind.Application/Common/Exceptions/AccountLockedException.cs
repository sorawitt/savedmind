namespace SavedMind.Application.Common.Exceptions;

public class AccountLockedException : ApplicationException
{
    public DateTime LockoutEnd { get; }

    public AccountLockedException(DateTime lockoutEnd)
        : base($"Account is locked until {lockoutEnd:u}.")
    {
        LockoutEnd = lockoutEnd;
    }
}