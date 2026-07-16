using FlowSync.Application.Contexts;
using FlowSync.Application.Models;
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
                OwnerId = userId,
                NormalizedName = request.Name.Trim().ToUpperInvariant(),
                CreatedAt = DateTime.UtcNow
            };
            _context.Workspaces.Add(newWorkspace);
            await _context.SaveChangesAsync(token);
            return true;
        }

        public async Task<bool> WorkspaceExistsByNameAsync(string name, Guid userId, CancellationToken token)
        {
            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.NormalizedName == name && w.OwnerId == userId, token);

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
            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.Id == request.Id, token);

            if (workspace == null || workspace.OwnerId != userId)
            {
                return false;
            }

            workspace.Name = request.Name;
            workspace.Description = request.Description;
            workspace.NormalizedName = request.Name.Trim().ToUpperInvariant();

            _context.Workspaces.Update(workspace);
            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<bool> IsWorkspaceOwnerAsync(Guid id, Guid userId, CancellationToken token)
        {
            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.Id == id, token);

            if (workspace == null || workspace.OwnerId != userId)
            {
                return false;
            }

            return true;
        }
    }
}
