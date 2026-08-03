using FlowSync.Application.Repositories;
using FlowSync.Contracts.Enums;

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

            return membership.Role == WorkspaceRole.Admin || (membership.Role == WorkspaceRole.Owner && ownerMember?.UserId == userId);
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

            return membership.Role == WorkspaceRole.Owner && ownerMember?.UserId == userId.Value;
        }

        public async Task<bool> CanInvite(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var membership = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);
            if (membership == null) { return false; }
            var ownerMember = membership.Workspace.Members
                .FirstOrDefault(m => m.Role == WorkspaceRole.Owner);
            if (ownerMember == null) { return false; }
            return membership.Role == WorkspaceRole.Admin || (membership.Role == WorkspaceRole.Owner && ownerMember?.UserId == userId.Value);
        }

        public async Task<bool> CanCancelInvitation(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var membership = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);
            if (membership == null) { return false; }
            var ownerMember = membership.Workspace.Members
                .FirstOrDefault(m => m.Role == WorkspaceRole.Owner);
            if (ownerMember == null) { return false; }
            return membership.Role == WorkspaceRole.Admin || (membership.Role == WorkspaceRole.Owner && ownerMember?.UserId == userId.Value);
        }

        public async Task<bool> CanViewPendingInvitations(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var membership = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);
            if (membership == null) { return false; }
            var ownerMember = membership.Workspace.Members
                .FirstOrDefault(m => m.Role == WorkspaceRole.Owner);
            if (ownerMember == null) { return false; }
            return membership.Role == WorkspaceRole.Admin || (membership.Role == WorkspaceRole.Owner && ownerMember?.UserId == userId.Value);
        }

        public async Task<bool> CanChangeRole(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var membership = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);
            if (membership == null) { return false; }
            var ownerMember = membership.Workspace.Members
                .FirstOrDefault(m => m.Role == WorkspaceRole.Owner);
            if (ownerMember == null) { return false; }
            return membership.Role == WorkspaceRole.Admin || (membership.Role == WorkspaceRole.Owner && ownerMember?.UserId == userId.Value);
        }

        public async Task<bool> CanRemoveWorkspaceMember(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var membership = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);
            if (membership == null) { return false; }
            var ownerMember = membership.Workspace.Members
                .FirstOrDefault(m => m.Role == WorkspaceRole.Owner);
            if (ownerMember == null) { return false; }
            return membership.Role == WorkspaceRole.Admin || (membership.Role == WorkspaceRole.Owner && ownerMember?.UserId == userId.Value);
        }
    }
}
