using FlowSync.Application.Contexts;
using FlowSync.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowSync.Application.Repositories
{
    public class AuthRepository: IAuthRepository
    {
        private ApplicationDbContext _context;
        public AuthRepository(ApplicationDbContext context) {
            _context = context;
        }
        public async Task<bool> Register(User user, CancellationToken token) {
            await _context.Users.AddAsync(user, token);
            await _context.SaveChangesAsync(token);
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
    }
}
