using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Repositories
{
    public interface IWorkspaceInvitationRepository
    {
        Task<bool> InvitationExistsByEmailAsync(string email, Guid workspaceId, CancellationToken token);
        Task<bool> CreateWorkspaceInvitationAsync(InviteToWorkspaceRequest request, Guid invitedByUserId, CancellationToken token);
        Task<bool> AcceptWorkspaceInvitationAsync(AcceptWorkspaceInvitationRequest request, string email, Guid userId, CancellationToken token);
        Task<bool> DeclineWorkspaceInvitationAsync(DeclineWorkspaceInvitationRequest request, string email, Guid userId, CancellationToken token);
        Task<bool> CancelWorkspaceInvitationAsync(CancelWorkspaceInvitationRequest request, CancellationToken token);
        Task<WorkspaceInvitation?> GetWorkspaceInvitationByTokenAndEmailAsync(string InvitationToken, string email, CancellationToken token);
        Task<WorkspaceInvitation?> GetWorkspaceInvitationByIdAsync(Guid id, CancellationToken token);
        Task<IEnumerable<WorkspaceInvitation>> GetPendingWorkspaceInvitationsAsync(GetPendingWorkspaceInvitationsRequest request, CancellationToken token);
    }
}
