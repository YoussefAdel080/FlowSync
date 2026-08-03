namespace FlowSync.Application.Services
{
    public interface IWorkspaceAuthorizationService
    {
        Task<bool> CanView(CancellationToken token);
        Task<bool> CanUpdate(Guid workspaceId, CancellationToken token);
        Task<bool> CanDelete(Guid workspaceId, CancellationToken token);
        Task<bool> CanInvite(Guid workspaceId, CancellationToken token);
        Task<bool> CanCancelInvitation(Guid workspaceId, CancellationToken token);
        Task<bool> CanViewPendingInvitations(Guid workspaceId, CancellationToken token);
        Task<bool> CanChangeRole(Guid workspaceId, CancellationToken token);
        Task<bool> CanRemoveWorkspaceMember(Guid workspaceId, CancellationToken token);
        Task<bool> CanLeaveWorkspace(Guid workspaceId, CancellationToken token);
    }
}
