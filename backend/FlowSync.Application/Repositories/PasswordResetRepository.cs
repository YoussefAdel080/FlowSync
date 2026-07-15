using FlowSync.Application.Contexts;
using FlowSync.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowSync.Application.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthRepository _authRepository;

        public PasswordResetRepository(ApplicationDbContext context, IAuthRepository authRepository)
        {
            _context = context;
            _authRepository = authRepository;
        }

        public async Task InvalidateActiveResetsAsync(Guid userId, CancellationToken token)
        {
            var activeResets = await _context.PasswordResets
                .Where(pr => pr.UserId == userId && !pr.IsUsed)
                .ToListAsync(token);

            foreach (var reset in activeResets)
            {
                reset.IsUsed = true;
            }

            if (activeResets.Count > 0)
            {
                await _context.SaveChangesAsync(token);
            }
        }

        public async Task<bool> AddPasswordResetAsync(Guid userId, string otp, CancellationToken token)
        {
            var passwordReset = new PasswordReset
            {
                UserId = userId,
                OtpCode = otp,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            };

            await _context.PasswordResets.AddAsync(passwordReset, token);
            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<bool> OtpExistsAsync(string otp, CancellationToken token)
        {
            var result = await _context.PasswordResets.FirstOrDefaultAsync(pr => pr.OtpCode == otp, token);
            return result != null;
        }

        public async Task<bool> OtpExpiredAsync(string otp, CancellationToken token)
        {
            var result = await _context.PasswordResets.FirstOrDefaultAsync(pr => pr.OtpCode == otp, token);
            return result?.IsExpired ?? false;
        }

        public async Task<bool> OtpUsedAsync(string otp, CancellationToken token)
        {
            var result = await _context.PasswordResets.FirstOrDefaultAsync(pr => pr.OtpCode == otp, token);
            return result?.IsUsed ?? false;
        }

        public async Task<bool> OtpBelongsToUserAsync(string email, string otp, CancellationToken token)
        {
            var user = await _authRepository.GetUserByEmailAsync(email, token);

            if (user == null)
            {
                return false;
            }

            var result = await _context.PasswordResets
                .FirstOrDefaultAsync(pr => pr.OtpCode == otp && pr.UserId == user.Id, token);

            return result != null;
        }

        public async Task<bool> ResetPasswordAsync(string email, string otp, string hashedPassword, CancellationToken token)
        {
            var user = await _authRepository.GetUserByEmailAsync(email, token);

            if (user == null)
            {
                return false;
            }

            var passwordReset = await _context.PasswordResets
                .FirstOrDefaultAsync(pr => pr.OtpCode == otp && pr.UserId == user.Id, token);

            if (passwordReset == null || passwordReset.IsExpired || passwordReset.IsUsed)
            {
                return false;
            }

            user.Password = hashedPassword;
            passwordReset.IsUsed = true;

            var otherActiveResets = await _context.PasswordResets
                .Where(pr => pr.UserId == user.Id && !pr.IsUsed && pr.Id != passwordReset.Id)
                .ToListAsync(token);

            foreach (var reset in otherActiveResets)
            {
                reset.IsUsed = true;
            }

            await _context.SaveChangesAsync(token);

            return true;
        }
    }
}
