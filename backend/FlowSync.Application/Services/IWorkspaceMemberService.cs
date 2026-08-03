using FlowSync.Application.Common.Pagination;
using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Services
{
    public interface IWorkspaceMemberService
    {
        Task<PaginationResult<WorkspaceMember>> GetWorkspaceMembersAsync(Guid WorkspaceId ,GetWorkspaceMembersRequest request ,CancellationToken token);
        Task<bool> ChangeWorkspaceMemberRoleAsync(Guid workspaceId, ChangeWorkspaceMemberRoleRequest request, CancellationToken token);
        Task<bool> RemoveWorkspaceMemberAsync(Guid workspaceId, RemoveWorkspaceMemberRequest request, CancellationToken token);
    }
}
