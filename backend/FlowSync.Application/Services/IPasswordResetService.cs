using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Services
{
    public interface IPasswordResetService
    {
        Task<bool> SendPasswordResetEmailAsync(string email, CancellationToken token);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken token);
    }
}
