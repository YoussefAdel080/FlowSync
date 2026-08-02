using FlowSync.Contracts.Enums;

namespace FlowSync.Contracts.Responses
{
    public class WorkspaceMemberResponse
    {
        public Guid Id { get; set; }
        public WorkspaceRole Role { get; set; }
        public Guid? InvitationId { get; set; }
        public InvitedByResponse? InvitedBy { get; set; }
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }

    public class InvitedByResponse
    {
        public Guid? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public WorkspaceRole? Role { get; set; }
    }
}
