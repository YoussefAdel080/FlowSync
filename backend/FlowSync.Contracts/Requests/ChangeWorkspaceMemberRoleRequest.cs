using FlowSync.Contracts.Enums;

namespace FlowSync.Contracts.Requests
{
    public class ChangeWorkspaceMemberRoleRequest
    {
        public AllowedWorkspaceRole Role { get; set; }
    }
}
