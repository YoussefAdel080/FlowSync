using FlowSync.Contracts.Enums;

namespace FlowSync.Application.Models
{
    public class WorkspaceMember
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid UserId { get; set; }
        public WorkspaceRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public Guid? WorkspaceInvitationId { get; set; }
        public WorkspaceInvitation? WorkspaceInvitation { get; set; } = null!;
        public User User { get; set; } = null!;
        public Workspace Workspace { get; set; } = null!;
    }
}
