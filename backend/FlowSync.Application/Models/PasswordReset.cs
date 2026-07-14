using System.ComponentModel.DataAnnotations.Schema;

namespace FlowSync.Application.Models
{
    public class PasswordReset
    {
        public Guid Id { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }
}
