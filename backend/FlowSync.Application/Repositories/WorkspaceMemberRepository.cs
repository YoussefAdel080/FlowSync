using FlowSync.Application.Common.Pagination;
using FlowSync.Application.Common.Sorting;
using FlowSync.Application.Contexts;
using FlowSync.Application.Models;
using FlowSync.Contracts.Enums;
using FlowSync.Contracts.Requests;
using FlowSync.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

namespace FlowSync.Application.Repositories
{
    public class WorkspaceMemberRepository : IWorkspaceMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkspaceMemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResult<WorkspaceMember>> GetWorkspaceMembersAsync(Guid workspaceId, GetWorkspaceMembersRequest request, CancellationToken token)
        {
            var query = _context.WorkspaceMembers
                .AsNoTracking()
                .AsQueryable()
                .Where(wm => wm.WorkspaceId == workspaceId);
            
            //Filtering
            if (!string.IsNullOrEmpty(request.MemberName))
            {
                query = query.Where(wm => 
                    EF.Functions.Like(wm.User.FirstName, $"%{request.MemberName}%") 
                    || EF.Functions.Like(wm.User.LastName, $"%{request.MemberName}%")
                );
            }

            if (request.Role.HasValue)
            {
                query = query.Where(wm =>
                    wm.Role == request.Role
                );
            }

            if (request.JoinedAtFrom.HasValue)
            {
                query = query.Where(wm =>
                    wm.JoinedAt >= request.JoinedAtFrom.Value
                );
            }

            if(request.JoinedAtTo.HasValue)
            {
                query = query.Where(wm =>
                    wm.JoinedAt <= request.JoinedAtTo.Value
                );
            }

            //Sorting
            if(request.SortByValue.HasValue)
            {
                switch (request.SortByValue.Value)
                {
                    case WorkspaceMemberSortBy.JoinedAt:
                        query = query.OrderByField(wm => wm.JoinedAt, request.Sort);
                        break;

                    case WorkspaceMemberSortBy.Role:
                        query = query.OrderByField(wm => wm.Role, request.Sort);
                        break;

                    case WorkspaceMemberSortBy.Name:
                        query = query
                            .OrderByField(wm => wm.User.FirstName, request.Sort)
                            .ThenByField(wm => wm.User.LastName, request.Sort);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.SortByValue));
                }
            }

            //Including
            query = query
                .Include(wm => wm.WorkspaceInvitation)
                    .ThenInclude(wi => wi.InvitedBy)
                .Include(wm => wm.User);

            //Pagintaion
            var totalCount = await query.CountAsync(token);
            query = query.Paginate(request.PageNumber, request.PageSize);


            var members = await query.ToListAsync(token);

            return new PaginationResult<WorkspaceMember>
            {
                Items = members,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int) (double)(totalCount + request.PageSize - 1) / request.PageSize
            };

        }

        public async Task<bool> ChangeWorkspaceMemberRoleAsync(Guid workspaceId, Guid memberId, ChangeWorkspaceMemberRoleRequest request, CancellationToken token)
        {
            var member = await _context.WorkspaceMembers.FirstOrDefaultAsync(w => w.WorkspaceId == workspaceId && w.Id == memberId, token);

            if (member is null) return false;

            member.Role = (WorkspaceRole)request.Role;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveWorkspaceMemberAsync(Guid workspaceId, Guid memberId, CancellationToken token)
        {
            var affectedRows = await _context.WorkspaceMembers
                .Where(wm => wm.WorkspaceId == workspaceId && wm.Id == memberId)
                .ExecuteDeleteAsync(token);

            return affectedRows > 0;
        }

        public async Task<bool> LeaveWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken token)
        {
            var affectedRows = await _context.WorkspaceMembers
                .Where(wm => wm.WorkspaceId == workspaceId && wm.UserId == userId)
                .ExecuteDeleteAsync(token);

            return affectedRows > 0;
        }

        public async Task<WorkspaceMemberResponse?> GetWorkspaceMemberByIdAsync(Guid memberId, CancellationToken token)
        {
            var member = await _context.WorkspaceMembers
                .AsNoTracking()
                .Select(wm => new WorkspaceMemberResponse
                {
                    Id = wm.Id,
                    Role = wm.Role,
                    InvitationId = wm.WorkspaceInvitationId,
                    InvitedBy = wm.WorkspaceInvitation != null 
                    ? new InvitedByResponse
                    {
                        Id = wm.WorkspaceInvitation.InvitedById,
                        DisplayName = wm.WorkspaceInvitation.InvitedBy.User.DisplayName,
                        Email = wm.WorkspaceInvitation.InvitedBy.User.Email,
                        Role = wm.WorkspaceInvitation.InvitedBy.Role
                    } : null,
                    UserId = wm.UserId,
                    DisplayName = wm.User.DisplayName,
                    Email = wm.User.Email,
                    JoinedAt = wm.JoinedAt,
                })
                .FirstOrDefaultAsync(wm => wm.Id == memberId);

            return member;
        }

        public async Task<WorkspaceRole?> GetWorkspaceMemberRoleAsync(Guid workspaceId, Guid userId, CancellationToken token)
        {
            var role = await _context.WorkspaceMembers
                .AsNoTracking()
                .Where(wm => wm.UserId == userId && wm.WorkspaceId == workspaceId)
                .Select(wm => wm.Role)
                .FirstOrDefaultAsync(token);

            return role;
        }

        public async Task<WorkspaceRole?> GetWorkspaceMemberRoleAsync(Guid memberId, CancellationToken token)
        {
            var role = await _context.WorkspaceMembers
                .AsNoTracking()
                .Where(wm => wm.Id == memberId)
                .Select(wm => wm.Role)
                .FirstOrDefaultAsync(token);

            return role;
        }
    }
}
