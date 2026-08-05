using FlowSync.Application.Common.Pagination;
using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;
using Microsoft.EntityFrameworkCore;

namespace FlowSync.Application.Repositories
{
    public interface IWorkspaceMemberRepository
    {
        Task<PaginationResult<WorkspaceMember>> GetWorkspaceMembersAsync(Guid workspaceId, GetWorkspaceMembersRequest request, CancellationToken token);
        Task<bool> ChangeWorkspaceMemberRoleAsync(Guid workspaceId ,Guid memberId, ChangeWorkspaceMemberRoleRequest request, CancellationToken token);
        Task<bool> RemoveWorkspaceMemberAsync(Guid workspaceId, Guid memberId, CancellationToken token);
        Task<bool> LeaveWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken token);
        Task<WorkspaceMember?> GetWorkspaceMemberByIdAsync(Guid memberId, CancellationToken token);
    }
}
