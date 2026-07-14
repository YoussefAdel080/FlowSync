using FlowSync.Application.Repositories;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator(IPasswordResetRepository passwordResetRepository)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP is required.")
                .Length(6).WithMessage("OTP must be 6 characters long.")
                .MustAsync(async (otp, cancellationToken) =>
                    await passwordResetRepository.OtpExistsAsync(otp, cancellationToken))
                .WithMessage("Invalid reset code.")
                .MustAsync(async (otp, cancellationToken) =>
                    !await passwordResetRepository.OtpExpiredAsync(otp, cancellationToken))
                .WithMessage("Reset code has expired.")
                .MustAsync(async (otp, cancellationToken) =>
                    !await passwordResetRepository.OtpUsedAsync(otp, cancellationToken))
                .WithMessage("Reset code has already been used.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New Password Is Required.")
                .MinimumLength(8).WithMessage("Password Must Be At Least 8 Characters Long.")
                .Matches(@"[A-Z]").WithMessage("Password Must Contain At Least One Uppercase Letter.")
                .Matches(@"[a-z]").WithMessage("Password Must Contain At Least One Lowercase Letter.")
                .Matches(@"[0-9]").WithMessage("Password Must Contain At Least One Number.")
                .Matches(@"([^a-zA-Z0-9])").WithMessage("Password Must Contain At Least One Special Character.");

            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                    await passwordResetRepository.OtpBelongsToUserAsync(
                        request.Email,
                        request.Otp,
                        cancellationToken))
                .WithMessage("Invalid reset code.");
        }
    }
}
