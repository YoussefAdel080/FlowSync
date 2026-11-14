using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Services
{
    public interface IWorkspaceService
    {
        Task<bool> CreateWorkspaceAsync(CreateWorkspaceRequest request, Guid userId, CancellationToken token);
        Task<bool> UpdateWorkspaceAsync(UpdateWorkspaceRequest request, Guid userId, CancellationToken token);
        Task<bool> DeleteWorkspaceAsync(DeleteWorkspaceRequest request, Guid userId, CancellationToken token);
        Task<IEnumerable<Workspace>> GetMyWorkspacesAsync(Guid userId, CancellationToken token);
        Task<Workspace?> GetWorkspaceByIdAsync(Guid id, Guid userId, CancellationToken token);
    }
}
