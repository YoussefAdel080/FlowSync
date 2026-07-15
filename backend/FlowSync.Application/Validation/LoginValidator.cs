using FlowSync.Contracts.Requests;
using FluentValidation;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email Address Is Required.")
            .EmailAddress().WithMessage("Please Enter A valid EMail Address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password Is Required.")
            .MinimumLength(8).WithMessage("Password Must Be At Least 8 Characters Long.");
    }
}