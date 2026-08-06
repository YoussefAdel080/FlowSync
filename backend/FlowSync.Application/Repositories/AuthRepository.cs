using FlowSync.Application.Contexts;
using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.Results;

namespace FlowSync.Application.Repositories
{
    public class AuthRepository: IAuthRepository
    {
        private ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AuthRepository(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        public async Task<bool> Register(User user, CancellationToken token) {
            await _context.Users.AddAsync(user, token);
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken token)
        {
            var result = await _context.Users
                .Where(user => user.Email == email)
                .ToListAsync();

            return result.Count != 0;
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken token)
        {
            var result = await _context.Users
                .FirstOrDefaultAsync(user => user.Email == email, token);

            return result;
        }

        public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken token)
        {
            var result = await _context.Users
                .FirstOrDefaultAsync(user => user.Id == id, token);

            return result;
        }

        public async Task<bool> UpdateUserProfileAsync(User user,UpdateProfileRequest request, CancellationToken token)
        {
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await _context.SaveChangesAsync(token);
            return true;
        }

        public async Task<bool> IsUserVerifiedAsync(string email, CancellationToken token)
        {
            var result = await _context.Users
                .FirstOrDefaultAsync(user => user.Email == email, token);

            return result?.IsEmailVerified ?? false;
        }

        public async Task<bool> ChangePassword(Guid userId, ChangePasswordRequest request, CancellationToken token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, token);
            
            if(user == null) return false;

            var isCurrentPasswordCorrect = _passwordHasher.VerifyHashedPassword(user, user.Password, request.CurrentPassword);
            if (!(isCurrentPasswordCorrect == PasswordVerificationResult.Success))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.CurrentPassword), "Current password is incorrect.")
                });
            }


            var isNewPasswordDifferent = _passwordHasher.VerifyHashedPassword(user, user.Password, request.NewPassword);
            if (isNewPasswordDifferent == PasswordVerificationResult.Success)
            {
                throw new ValidationException(new[]
    {
                    new ValidationFailure(nameof(request.NewPassword),
                        "The new password must be different from the current password.")
                });
            }

            var hashedPassword = _passwordHasher.HashPassword(user, request.NewPassword);

            user.Password = hashedPassword;

            await _context.SaveChangesAsync(token);

            return true;
        }
    }
}
