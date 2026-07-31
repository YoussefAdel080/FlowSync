using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Services
{
    public interface IWorkspaceInvitationService
    {
        Task<bool> CreateWorkspaceInvitationAsync(InviteToWorkspaceRequest requset, Guid userId, CancellationToken token);
        Task<bool> AcceptWorkspaceInvitationAsync(AcceptWorkspaceInvitationRequest request, Guid userId, CancellationToken token);
        Task<bool> DeclineWorkspaceInvitationAsync(DeclineWorkspaceInvitationRequest request, Guid userId, CancellationToken token);
        Task<bool> CancelWorkspaceInvitationAsync(CancelWorkspaceInvitationRequest request, Guid userId, CancellationToken token);
        Task<IEnumerable<WorkspaceInvitation>> GetPendingWorkspaceInvitationsAsync(GetPendingWorkspaceInvitationsRequest request, Guid userId, CancellationToken token);
    }
}
