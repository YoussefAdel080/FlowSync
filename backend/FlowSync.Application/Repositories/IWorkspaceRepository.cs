using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Repositories
{
    public interface IWorkspaceRepository
    {
        Task<bool> CreateWorkspaceAsync(CreateWorkspaceRequest request, Guid userId, CancellationToken token);
        Task<bool> UpdateWorkspaceAsync(UpdateWorkspaceRequest request, Guid userId, CancellationToken token);
        Task<bool> WorkspaceExistsByNameAsync(string name, Guid userId, CancellationToken token);
        Task<bool> WorkspaceExistsByIdAsync(Guid id, CancellationToken token);
        Task<bool> IsWorkspaceOwnerAsync(Guid id, Guid userId,CancellationToken token);
    }
}
