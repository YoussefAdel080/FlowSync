using FlowSync.Application.Models;

namespace FlowSync.Application.Services
{
    public interface IAuthService
    {
        Task<bool> Register(User user, CancellationToken token);
    }
}
