using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Services
{
    public interface IWorkspaceService
    {
        Task<bool> CreateWorkspaceAsync(CreateWorkspaceRequest request, Guid userId, CancellationToken token);
        Task<bool> UpdateWorkspaceAsync(UpdateWorkspaceRequest request, Guid userId, CancellationToken token);
    }
}
