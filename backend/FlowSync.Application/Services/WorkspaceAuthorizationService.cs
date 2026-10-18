using FlowSync.Application.Repositories;
using FlowSync.Contracts.Enums;

namespace FlowSync.Application.Services
{
    public class WorkspaceAuthorizationService : IWorkspaceAuthorizationService
    {
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly ICurrentUserService _currentUserService;

        public WorkspaceAuthorizationService(IWorkspaceMemberRepository workspaceMemberRepository, ICurrentUserService currentUserService)
        {
            _workspaceMemberRepository = workspaceMemberRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> CanView(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;

            if (userId == null) { return false; }

            var role = await _workspaceMemberRepository.GetWorkspaceMemberRoleAsync(workspaceId, userId.Value, token);

            return role != null;
        }

        public async Task<bool> CanUpdate(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;

            if (userId == null) { return false; }

            var role = await _workspaceMemberRepository.GetWorkspaceMemberRoleAsync(workspaceId, userId.Value, token);

            if(role == null) { return false; }

            return role == WorkspaceRole.Admin || role == WorkspaceRole.Owner;
        }
        public async Task<bool> CanDelete(Guid workspaceId,CancellationToken token)
        {
            var userId = _currentUserService.UserId;

            if (userId == null) { return false; }

            var role = await _workspaceMemberRepository.GetWorkspaceMemberRoleAsync(workspaceId, userId.Value, token);

            if (role == null) { return false; }

            return role == WorkspaceRole.Owner;
        }

        public async Task<bool> CanInvite(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }

            var role = await _workspaceMemberRepository.GetWorkspaceMemberRoleAsync(workspaceId, userId.Value, token);
            if (role == null) { return false; }

            return role == WorkspaceRole.Admin || role == WorkspaceRole.Owner;
        }

        public async Task<bool> CanCancelInvitation(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var role = await _workspaceMemberRepository.GetWorkspaceMemberRoleAsync(workspaceId, userId.Value, token);
            if (role == null) { return false; }
            
            return role == WorkspaceRole.Admin || role == WorkspaceRole.Owner;
        }

        public async Task<bool> CanViewPendingInvitations(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var role = await _workspaceMemberRepository.GetWorkspaceMemberRoleAsync(workspaceId, userId.Value, token);
            if (role == null) { return false; }
            
            return role == WorkspaceRole.Admin || role == WorkspaceRole.Owner;
        }

        public async Task<bool> CanChangeRole(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var role = await _workspaceMemberRepository.GetWorkspaceMemberRoleAsync(workspaceId, userId.Value, token);
            if (role == null) { return false; }
            
            return role == WorkspaceRole.Admin || role == WorkspaceRole.Owner;
        }

        public async Task<bool> CanRemoveWorkspaceMember(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var role = await _workspaceMemberRepository.GetWorkspaceMemberRoleAsync(workspaceId, userId.Value, token);
            if (role == null) { return false; }
            
            return role == WorkspaceRole.Admin || role == WorkspaceRole.Owner;
        }

        public async Task<bool> CanLeaveWorkspace(Guid workspaceId, CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) { return false; }
            var role = await _workspaceMemberRepository.GetWorkspaceMemberRoleAsync(workspaceId, userId.Value, token);
            if (role == null) { return false; }
            
            return !(role == WorkspaceRole.Owner);
        }
    }
}
