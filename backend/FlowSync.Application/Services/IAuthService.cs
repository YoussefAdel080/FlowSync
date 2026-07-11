using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;
using FlowSync.Contracts.Responses;

namespace FlowSync.Application.Services
{
    public interface IAuthService
    {
        Task<bool> Register(User user, CancellationToken token);
        Task<bool> VerifyEmail(VerifyEmailRequest request, CancellationToken token);
        Task<LoginResponseData> Login(LoginRequest request, CancellationToken token);
        Task<LoginResponseData?> Refresh(RefreshRequest request, CancellationToken token);
        Task<bool> Logout(LogoutRequest request, CancellationToken cancellationToken);
        Task<GetProfileResponse?> GetProfile(Guid userId,CancellationToken cancellationToken);
        Task<bool> UpdateProfile(Guid userId, UpdateProfileRequest requset,CancellationToken cancellationToken);
    }
}
