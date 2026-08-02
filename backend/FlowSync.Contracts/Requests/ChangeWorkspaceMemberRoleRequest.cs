using FlowSync.Contracts.Enums;

namespace FlowSync.Contracts.Requests
{
    public class ChangeWorkspaceMemberRoleRequest
    {
        public Guid Id { get; set; }
        public AllowedWorkspaceRole Role { get; set; }
    }
}
