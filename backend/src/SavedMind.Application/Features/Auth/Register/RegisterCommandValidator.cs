using FluentValidation;

namespace SavedMind.Application.Features.Auth.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"\d").WithMessage("Password must contain at least 1 number.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least 1 uppercase letter.");
    }
}