namespace FlowSync.Application.Repositories
{
    public interface IEmailVerificationRepository
    {
        Task<bool> AddEmailVerificationAsync(Guid userId, string otp, CancellationToken token);
        Task<bool> OtpExistsAsync(string otp, CancellationToken token);
        Task<bool> OtpExpiredAsync(string otp, CancellationToken token);
        Task<bool> OtpUsedAsync(string otp, CancellationToken token);
        Task<bool> OtpBelongsToUserAsync(string email, string otp, CancellationToken token);
        Task<bool> VerifyEmailAsync(string email, string otp, CancellationToken token);
    }
}
