using FlowSync.Application.Contexts;
using FlowSync.Application.Enums;
using FlowSync.Application.Models;
using FlowSync.Contracts.Enums;
using FlowSync.Contracts.Requests;
using Microsoft.EntityFrameworkCore;

namespace FlowSync.Application.Repositories
{
    public class WorkspaceRepository: IWorkspaceRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkspaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateWorkspaceAsync(CreateWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            var newWorkspace = new Workspace
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                NormalizedName = request.Name.Trim().ToUpperInvariant(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Workspaces.Add(newWorkspace);
            await _context.SaveChangesAsync(token);

            var ownerWorkspaceMembership = await CreateWorkspaceMemberAsync(newWorkspace.Id, userId, WorkspaceRole.Owner, token);

            return true;
        }

        public async Task<bool> WorkspaceExistsByNameAsync(string name, Guid userId, CancellationToken token)
        {
            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.NormalizedName == name, token);

            return workspace != null;
        }

        public async Task<bool> WorkspaceExistsByIdAsync(Guid id, CancellationToken token)
        {
            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.Id == id, token);

            return workspace != null;
        }

        public async Task<bool> UpdateWorkspaceAsync(UpdateWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            var workspaceOwnerMembership = await _context.WorkspaceMembers
                .Include(wm => wm.Workspace)
                .FirstOrDefaultAsync(wm => 
                wm.WorkspaceId == request.Id &&
                wm.UserId == userId &&
                wm.Role == WorkspaceRole.Owner, token);

            if (workspaceOwnerMembership == null || workspaceOwnerMembership.Workspace == null)
            {
                return false;
            }

            var workspace = workspaceOwnerMembership.Workspace;
                
            workspace.Name = request.Name;
            workspace.Description = request.Description;
            workspace.NormalizedName = request.Name.Trim().ToUpperInvariant();

            _context.Workspaces.Update(workspace);
            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<bool> IsWorkspaceOwnerAsync(Guid id, Guid userId, CancellationToken token)
        {
            var workspaceOwnerMembership = await _context.WorkspaceMembers
                .AnyAsync(wm =>
                wm.WorkspaceId == id &&
                wm.UserId == userId &&
                wm.Role == WorkspaceRole.Owner,
                token);

            return workspaceOwnerMembership;
        }

        public async Task<bool> DeleteWorkspaceAsync(DeleteWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            var workspaceOwnerMembership = await _context.WorkspaceMembers
                .Include(wm => wm.Workspace)
                .FirstOrDefaultAsync(wm =>
                wm.WorkspaceId == request.Id &&
                wm.UserId == userId &&
                wm.Role == WorkspaceRole.Owner, token);

            if (workspaceOwnerMembership == null || workspaceOwnerMembership.Workspace == null)
            {
                return false;
            }

            var workspace = workspaceOwnerMembership.Workspace;

            _context.Workspaces.Remove(workspace);
            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<IEnumerable<Workspace>> GetMyWorkspacesAsync(Guid userId, CancellationToken token)
        {
            return await _context.Workspaces
                .AsNoTracking()
                .Where(w => w.Members.Any(m =>
                    m.UserId == userId &&
                    m.Role == WorkspaceRole.Owner))
                .Include(w => w.Members)
                    .ThenInclude(m => m.User)
                .ToListAsync(token);
        }

        public async Task<Workspace?> GetWorkspaceByIdAsync(Guid id, CancellationToken token)
        {
            var worksapceMmbership = await _context.WorkspaceMembers
                .AsNoTracking()
                .Include(wm => wm.Workspace)
                .FirstOrDefaultAsync(w => w.WorkspaceId == id, token);

            return worksapceMmbership?.Workspace;
        }

        public async Task<WorkspaceMember> CreateWorkspaceMemberAsync(Guid workspaceId, Guid userId, WorkspaceRole role, CancellationToken token)
        {
            var workspaceMembership = new WorkspaceMember
            {
                Id = Guid.NewGuid(),
                WorkspaceId = workspaceId,
                UserId = userId,
                Role = role,
                JoinedAt = DateTime.UtcNow,
            };

            await _context.WorkspaceMembers.AddAsync(workspaceMembership);
            await _context.SaveChangesAsync(token);

            return workspaceMembership;
        }

        public async Task<WorkspaceMember?> GetWorkspaceMembershipAsync(Guid workspaceId, Guid userId, CancellationToken token)
        {
            var membership = await _context.WorkspaceMembers
                .AsNoTracking()
                .Include(wm => wm.Workspace)
                    .ThenInclude(w => w.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(wm => wm.UserId == userId && wm.WorkspaceId == workspaceId);

            return membership;
        }

        public async Task<bool> IsWorkspaceMemberByEmailAsync(string email, Guid WorksapceId, CancellationToken token)
        {
            var membership = await _context.WorkspaceMembers
                .AsNoTracking()
                .Include(wm => wm.User)
                .FirstOrDefaultAsync(wm => wm.User.Email == email && wm.WorkspaceId == WorksapceId, token);
            
            return membership != null;
        }
    }
}
