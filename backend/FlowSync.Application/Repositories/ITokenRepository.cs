using FlowSync.Application.Models;

namespace FlowSync.Application.Repositories
{
    public interface ITokenRepository
    {
        Task<bool> SaveRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<bool> RefreshTokenExistsAsync(string token, CancellationToken cancellationToken);
        Task<bool> RefreshTokenRevokedAsync(string token, CancellationToken cancellationToken);
        Task<bool> RefreshTokenExpiredAsync(string token, CancellationToken cancellationToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);
        Task<RefreshToken?> RotateRefreshTokenAsync(string oldToken, RefreshToken newRefreshToken, CancellationToken cancellationToken);
        Task<bool> RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken);
        Task<bool> RevokeAllRefreshTokensForUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}
