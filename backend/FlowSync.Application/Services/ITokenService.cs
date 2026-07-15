using FlowSync.Application.Models;
using FlowSync.Contracts.Responses;

namespace FlowSync.Application.Services;
public interface ITokenService
{
    (string Token, DateTime Expiration) GenerateAccessToken(User user);

    (string Token, DateTime Expiration) GenerateRefreshToken();

    Task<bool> SaveRefreshTokenAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task<LoginResponseData?> RefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task<bool> LogoutAsync(string token, CancellationToken cancellationToken);
    Task<bool> RevokeAllRefreshTokensForUserAsync(Guid userId, CancellationToken cancellationToken);
}