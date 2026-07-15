
using FlowSync.Application.Contexts;
using FlowSync.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowSync.Application.Repositories
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthRepository _authRepository;

        public EmailVerificationRepository(ApplicationDbContext context, IAuthRepository authRepository)
        {
            _context = context;
            _authRepository = authRepository;
        }

        public async Task InvalidateActiveVerificationsAsync(Guid userId, CancellationToken token)
        {
            var activeVerifications = await _context.EmailVerifications
                .Where(ev => ev.UserId == userId && !ev.IsUsed)
                .ToListAsync(token);

            foreach (var verification in activeVerifications)
            {
                verification.IsUsed = true;
            }

            if (activeVerifications.Count > 0)
            {
                await _context.SaveChangesAsync(token);
            }
        }

        public async Task<bool> AddEmailVerificationAsync(Guid userId, string otp, CancellationToken token)
        {
            var emailVerification = new EmailVerification
            {
                UserId = userId,
                OtpCode = otp,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            };

            await _context.EmailVerifications.AddAsync(emailVerification, token);
            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<bool> OtpExistsAsync(string otp, CancellationToken token)
        {
            var result = await _context.EmailVerifications.FirstOrDefaultAsync(ev => ev.OtpCode == otp, token);
            return result != null;
        }

        public async Task<bool> OtpExpiredAsync(string otp, CancellationToken token)
        {
            var result = await _context.EmailVerifications.FirstOrDefaultAsync(ev => ev.OtpCode == otp, token);

            return result?.IsExpired ?? false;
        }

        public async Task<bool> OtpUsedAsync(string otp, CancellationToken token)
        {
            var result = await _context.EmailVerifications.FirstOrDefaultAsync(ev => ev.OtpCode == otp, token);

            return result?.IsUsed ?? false;
        }

        public async Task<bool> OtpBelongsToUserAsync(string email, string otp, CancellationToken token)
        {
            var user = await _authRepository.GetUserByEmailAsync(email, token);

            if (user == null) {
                return false;
            }

            var result = await _context.EmailVerifications.FirstOrDefaultAsync(ev => ev.OtpCode == otp && ev.UserId == user.Id, token);
            
            return result != null;
        }

        public async Task<bool> VerifyEmailAsync(string email, string otp, CancellationToken token)
        {
            var user = await _authRepository.GetUserByEmailAsync(email, token);

            if (user == null)
            {
                return false;
            }

            var result = await _context.EmailVerifications.FirstOrDefaultAsync(ev => ev.OtpCode == otp && ev.UserId == user.Id, token);

            if (result == null || result.IsExpired || result.IsUsed)
            {
                return false;
            }

            user.IsEmailVerified = true;
            result.IsUsed = true;

            var otherActiveVerifications = await _context.EmailVerifications
                .Where(ev => ev.UserId == user.Id && !ev.IsUsed && ev.Id != result.Id)
                .ToListAsync(token);

            foreach (var verification in otherActiveVerifications)
            {
                verification.IsUsed = true;
            }

            await _context.SaveChangesAsync(token);

            return true;
        }
    }
}
