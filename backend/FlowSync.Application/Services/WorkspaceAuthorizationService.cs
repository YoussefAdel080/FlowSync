
using FlowSync.Application.Enums;
using FlowSync.Application.Models;
using FlowSync.Application.Repositories;

namespace FlowSync.Application.Services
{
    public class WorkspaceAuthorizationService : IWorkspaceAuthorizationService
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly ICurrentUserService _currentUserService;

        public WorkspaceAuthorizationService(IWorkspaceRepository workspaceRepository, ICurrentUserService currentUserService)
        {
            _workspaceRepository = workspaceRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> CanView(CancellationToken token)
        {
            // a placeholder for further view access restrictions
            return true;
        }

        public async Task<bool> CanUpdate(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;

            if (userId == null) { return false; }

            var membership = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);

            if(membership == null) { return false; }

            var ownerMember = membership.Workspace.Members
                .FirstOrDefault(m => m.Role == WorkspaceRole.Owner);
            
            if (ownerMember == null) {return false; }

            return membership.Role == Enums.WorkspaceRole.Admin || (membership.Role == Enums.WorkspaceRole.Owner && ownerMember?.UserId == userId);
        }
        public async Task<bool> CanDelete(Guid workspaceId,CancellationToken token)
        {
            var userId = _currentUserService.UserId;

            if (userId == null) { return false; }

            var membership = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);

            if (membership == null) { return false; }

            var ownerMember = membership.Workspace.Members
                .FirstOrDefault(m => m.Role == WorkspaceRole.Owner);

            if (ownerMember == null) { return false; }

            return membership.Role == Enums.WorkspaceRole.Owner && ownerMember?.UserId == userId.Value;
        }

    }
}
