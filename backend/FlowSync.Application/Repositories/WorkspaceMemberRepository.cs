using FlowSync.Application.Common.Pagination;
using FlowSync.Application.Common.Sorting;
using FlowSync.Application.Contexts;
using FlowSync.Application.Models;
using FlowSync.Contracts.Enums;
using FlowSync.Contracts.Requests;
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
                PageSize = request.PageSize
            };

        }

        public async Task<bool> ChangeWorkspaceMemberRoleAsync(Guid workspaceId, ChangeWorkspaceMemberRoleRequest request, CancellationToken token)
        {
            var member = await _context.WorkspaceMembers.FirstOrDefaultAsync(w => w.WorkspaceId == workspaceId && w.Id == request.Id, token);

            if (member is null) return false;

            member.Role = (WorkspaceRole)request.Role;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveWorkspaceMemberAsync(Guid workspaceId, RemoveWorkspaceMemberRequest request, CancellationToken token)
        {
            var affectedRows = await _context.WorkspaceMembers
                .Where(wm => wm.WorkspaceId == workspaceId && wm.Id == request.Id)
                .ExecuteDeleteAsync(token);

            return affectedRows > 0;
        }
    }
}
