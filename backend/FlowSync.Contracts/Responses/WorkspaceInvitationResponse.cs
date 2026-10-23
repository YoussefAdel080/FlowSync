using FlowSync.Contracts.Enums;

namespace FlowSync.Contracts.Responses
{
    public class WorkspaceInvitationResponse
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid InvitedById { get; set; }
        public AllowedWorkspaceRole Role { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public WorkspaceInvitationStatus Status { get; set; }
        public InvitedByUserResponse InvitedBy { get; set; } = null!;
        public WorkspaceResponse Workspace { get; set; } = null!;
    }

    public class InvitedByUserResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public WorkspaceRole Role { get; set; }
    }
}
