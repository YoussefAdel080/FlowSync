using FlowSync.Application.Models;
using FlowSync.Application.Repositories;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class RegistrationValidator: AbstractValidator<User>
    {
        public IAuthRepository _authRepository;

        public RegistrationValidator(IAuthRepository authRepository)
        {
            _authRepository = authRepository;

            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name Is Required.")
            .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email Address Is Required.")
                .EmailAddress()
                .MustAsync(
                    async (email, token) =>
                    {
                        return !await _authRepository.EmailExistsAsync(email, token);
                    })
                .WithMessage("Email Already Exists");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password Is Required.")
                .MinimumLength(8).WithMessage("Password Must Be At Least 8 Characters Long.")
                .Matches(@"[A-Z]").WithMessage("Password Must Contain At Least One Uppercase Letter.")
                .Matches(@"[a-z]").WithMessage("Password Must Contain At Least One Lowercase Letter.")
                .Matches(@"[0-9]").WithMessage("Password Must Contain At Least One Number.")
                .Matches(@"([^a-zA-Z0-9])").WithMessage("Password Must Contain At Least One Special Character.");
        }
    }
}
