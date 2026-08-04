using FlowSync.Application.Common.Pagination;
using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;

namespace FlowSync.Application.Services
{
    public interface IWorkspaceMemberService
    {
        Task<PaginationResult<WorkspaceMember>> GetWorkspaceMembersAsync(Guid WorkspaceId ,GetWorkspaceMembersRequest request ,CancellationToken token);
        Task<bool> ChangeWorkspaceMemberRoleAsync(Guid workspaceId, Guid memberId, ChangeWorkspaceMemberRoleRequest request, CancellationToken token);
        Task<bool> RemoveWorkspaceMemberAsync(Guid workspaceId, Guid memberId, CancellationToken token);
        Task<bool> LeaveWorkspaceAsync(Guid workspaceId, CancellationToken token);
    }
}
