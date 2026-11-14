using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;
using FlowSync.Contracts.Responses;

namespace FlowSync.Application.Services
{
    public interface IWorkspaceInvitationService
    {
        Task<bool> CreateWorkspaceInvitationAsync(InviteToWorkspaceRequest requset, Guid userId, CancellationToken token);
        Task<bool> AcceptWorkspaceInvitationAsync(AcceptWorkspaceInvitationRequest request, Guid userId, CancellationToken token);
        Task<bool> DeclineWorkspaceInvitationAsync(DeclineWorkspaceInvitationRequest request, Guid userId, CancellationToken token);
        Task<bool> CancelWorkspaceInvitationAsync(CancelWorkspaceInvitationRequest request, Guid userId, CancellationToken token);
        Task<IEnumerable<WorkspaceInvitationResponse>> GetPendingWorkspaceInvitationsAsync(GetPendingWorkspaceInvitationsRequest request, Guid userId, CancellationToken token);
    }
}
