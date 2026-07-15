using FlowSync.Application.Repositories;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class VerifyEmailValidator: AbstractValidator<VerifyEmailRequest>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IEmailVerificationRepository _emailVerificationRepository;

        public VerifyEmailValidator(IAuthRepository authRepository, IEmailVerificationRepository emailVerificationRepository)
        {
            _authRepository = authRepository;
            _emailVerificationRepository = emailVerificationRepository;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MustAsync(async (email, cancellationToken) =>
                {
                    var emailExists = await _authRepository.EmailExistsAsync(email, cancellationToken);
                    return emailExists;
                }).MustAsync(async (email, cancellationToken) =>
                {
                    var emailVerified = await _authRepository.IsUserVerifiedAsync(email, cancellationToken);
                    return !emailVerified;
                }).WithMessage("User is already activated.");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP is required.")
                .Length(6).WithMessage("OTP must be 6 characters long.")
                .MustAsync(
                    async (otp, cancellationToken) =>
                    {
                        var otpExists = await _emailVerificationRepository.OtpExistsAsync(otp, cancellationToken);
                        return otpExists;
                    }
                )
                .MustAsync(
                    async (otp, cancellationToken) =>
                    {
                        var emailExpired = await _emailVerificationRepository.OtpExpiredAsync(otp, cancellationToken);
                        return !emailExpired;
                    }
                ).MustAsync(
                    async (otp, cancellationToken) =>
                    {
                        var emailUsed = await _emailVerificationRepository.OtpUsedAsync(otp, cancellationToken);
                        return !emailUsed;
                    }
                );

            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                {
                    return await _emailVerificationRepository
                        .OtpBelongsToUserAsync(
                            request.Email,
                            request.Otp,
                            cancellationToken);
                })
                .WithMessage("Invalid verification code.");
        }
    }
}
