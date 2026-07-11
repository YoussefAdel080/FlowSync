namespace FlowSync.Application.Services
{
    public interface IEmailVerificationService
    {
        Task<bool> SendVerificationEmailAsync(string email, Guid userId, CancellationToken token);
        Task<bool> VerifyEmailAsync(string email, string otp, CancellationToken token);
    }
}
