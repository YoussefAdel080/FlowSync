using System.ComponentModel.DataAnnotations.Schema;

namespace FlowSync.Application.Models
{
    public class EmailVerification
    {
        public Guid Id { get; set; }
        public string OtpCode { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }
}
