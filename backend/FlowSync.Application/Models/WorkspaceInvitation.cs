using FlowSync.Application.Enums;
using FlowSync.Contracts.Enums;

namespace FlowSync.Application.Models
{
    public class WorkspaceInvitation
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid InvitedById { get; set; }
        public WorkspaceRole Role { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public WorkspaceInvitationStatus Status {get; set;}
        public User InvitedBy { get; set; } = null!;
        public Workspace Workspace { get; set; } = null!;
    }
}
