using FlowSync.Application.Repositories;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class RefreshValidator: AbstractValidator<RefreshRequest>
    {
        private readonly ITokenRepository _tokenRepository;
        public RefreshValidator(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;

            RuleFor(x => x.RefreshToken)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Refresh Token Is Required.")
                .MustAsync(async (token, cancellation) =>
                    await _tokenRepository.RefreshTokenExistsAsync(token, cancellation))
                .WithMessage("Invalid Refresh Token.")
                .MustAsync(async (token, cancellation) =>
                    !await _tokenRepository.RefreshTokenRevokedAsync(token, cancellation))
                .WithMessage("Refresh Token Is Revoked.")
                .MustAsync(async (token, cancellation) =>
                    !await _tokenRepository.RefreshTokenExpiredAsync(token, cancellation))
                .WithMessage("Refresh Token Is Expired.");
        }
    }
}
