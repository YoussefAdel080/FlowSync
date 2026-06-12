using FlowSync.Application.Models;

namespace FlowSync.Application.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> Register(User user, CancellationToken token);
        Task<bool> EmailExistsAsync(string email, CancellationToken token);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken token);
    }
}
