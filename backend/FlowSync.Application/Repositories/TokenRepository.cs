using FlowSync.Application.Contexts;
using FlowSync.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowSync.Application.Repositories
{
    public class TokenRepository: ITokenRepository
    {
        private readonly ApplicationDbContext _context;

        public TokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            var result = await _context.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<bool> RefreshTokenExistsAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens.AnyAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task<bool> RefreshTokenRevokedAsync(string token, CancellationToken cancellationToken)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
            return refreshToken?.IsRevoked ?? false;
        }

        public async Task<bool> RefreshTokenExpiredAsync(string token, CancellationToken cancellationToken)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
            return refreshToken?.IsExpired ?? false;
        }

        public async Task<RefreshToken? > GetRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task<RefreshToken?> RotateRefreshTokenAsync(
            string oldToken,
            RefreshToken newRefreshToken,
            CancellationToken cancellationToken)
        {
            var existingToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == oldToken, cancellationToken);

            if (existingToken is null)
                return null;

            newRefreshToken.UserId = existingToken.UserId;

            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.ReplacedByToken = newRefreshToken;

            _context.RefreshTokens.Add(newRefreshToken);

            await _context.SaveChangesAsync(cancellationToken);

            return existingToken;
        }
    }
}
