using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current Password Is Required.");
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New Password Is Required.")
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("New password must be different from the current password.")
                .MinimumLength(8).WithMessage("Password Must Be At Least 8 Characters Long.")
                .Matches(@"[A-Z]").WithMessage("Password Must Contain At Least One Uppercase Letter.")
                .Matches(@"[a-z]").WithMessage("Password Must Contain At Least One Lowercase Letter.")
                .Matches(@"[0-9]").WithMessage("Password Must Contain At Least One Number.")
                .Matches(@"([^a-zA-Z0-9])").WithMessage("Password Must Contain At Least One Special Character.");
        }
    }
}
