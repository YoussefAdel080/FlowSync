using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> Register(User user, CancellationToken token);
        Task<bool> EmailExistsAsync(string email, CancellationToken token);
        Task<bool> IsUserVerifiedAsync(string email, CancellationToken token);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken token);
        Task<User?> GetUserByIdAsync(Guid id, CancellationToken token);
        Task<bool> UpdateUserProfileAsync(User user,UpdateProfileRequest request, CancellationToken token);
    }
}
