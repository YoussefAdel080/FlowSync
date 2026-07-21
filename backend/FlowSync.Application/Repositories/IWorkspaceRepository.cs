using FlowSync.Application.Enums;
using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Repositories
{
    public interface IWorkspaceRepository
    {
        Task<bool> CreateWorkspaceAsync(CreateWorkspaceRequest request, Guid userId, CancellationToken token);
        Task<bool> UpdateWorkspaceAsync(UpdateWorkspaceRequest request, Guid userId, CancellationToken token);
        Task<bool> DeleteWorkspaceAsync(DeleteWorkspaceRequest request, Guid userId, CancellationToken token);
        Task<bool> WorkspaceExistsByNameAsync(string name, Guid userId, CancellationToken token);
        Task<bool> WorkspaceExistsByIdAsync(Guid id, CancellationToken token);
        Task<bool> IsWorkspaceOwnerAsync(Guid id, Guid userId,CancellationToken token);
        Task<IEnumerable<Workspace>> GetMyWorkspacesAsync(Guid userId, CancellationToken token);
        Task<Workspace?> GetWorkspaceByIdAsync(Guid id, CancellationToken token);
        Task<WorkspaceMember> CreateWorkspaceMemberAsync(Guid workspaceId, Guid userId, WorkspaceRole role, CancellationToken token);
        Task<WorkspaceMember?> GetWorkspaceMembershipAsync(Guid workspaceId, Guid userId, CancellationToken token);
    }
}
