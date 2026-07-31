using FlowSync.Contracts.Enums;

namespace FlowSync.Contracts.Requests
{
    public class InviteToWorkspaceRequest
    {
        public Guid WorkspaceId { get; set; }
        public string Email { get; set; }
        public WorkspaceRole Role { get; set; }
    }
}
