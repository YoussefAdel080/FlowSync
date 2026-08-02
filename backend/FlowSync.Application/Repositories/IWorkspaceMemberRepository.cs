using FlowSync.Application.Common.Pagination;
using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Repositories
{
    public interface IWorkspaceMemberRepository
    {
        Task<PaginationResult<WorkspaceMember>> GetWorkspaceMembersAsync(Guid WorkspaceId, GetWorkspaceMembersRequest request, CancellationToken token);
    }
}
