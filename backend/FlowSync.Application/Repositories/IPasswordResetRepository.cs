namespace FlowSync.Application.Repositories
{
    public interface IPasswordResetRepository
    {
        Task<bool> AddPasswordResetAsync(Guid userId, string otp, CancellationToken token);
        Task InvalidateActiveResetsAsync(Guid userId, CancellationToken token);
        Task<bool> OtpExistsAsync(string otp, CancellationToken token);
        Task<bool> OtpExpiredAsync(string otp, CancellationToken token);
        Task<bool> OtpUsedAsync(string otp, CancellationToken token);
        Task<bool> OtpBelongsToUserAsync(string email, string otp, CancellationToken token);
        Task<bool> ResetPasswordAsync(string email, string otp, string hashedPassword, CancellationToken token);
    }
}
