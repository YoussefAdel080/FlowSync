using FlowSync.Application.Common.Pagination;
using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Repositories
{
    public interface IWorkspaceMemberRepository
    {
        Task<PaginationResult<WorkspaceMember>> GetWorkspaceMembersAsync(Guid workspaceId, GetWorkspaceMembersRequest request, CancellationToken token);
        Task<bool> ChangeWorkspaceMemberRoleAsync(Guid workspaceId,Guid userId , ChangeWorkspaceMemberRoleRequest request, CancellationToken token);
    }
}
