using FlowSync.Application.Common.Pagination;
using FlowSync.Application.Exceptions;
using FlowSync.Application.Models;
using FlowSync.Application.Repositories;
using FlowSync.Contracts.Enums;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Services
{
    public class WorkspaceMemberService : IWorkspaceMemberService
    {
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<GetWorkspaceMembersRequest> _getWorkspaceMembersValidator;
        private readonly IValidator<ChangeWorkspaceMemberRoleRequest> _changeWorkspaceMemberRoleValidator;
        private readonly IValidator<PaginationRequest> _paginationValidator;

        public WorkspaceMemberService(
            IWorkspaceMemberRepository workspaceMemberRepository,
            IValidator<GetWorkspaceMembersRequest> getWorkspaceMembersValidator,
            IValidator<PaginationRequest> paginationValidator,
            IWorkspaceRepository workspaceRepository,
            IValidator<ChangeWorkspaceMemberRoleRequest> changeWorkspaceMemberRoleValidator,
            IWorkspaceAuthorizationService workspaceAuthorizationService,
            ICurrentUserService currentUserService)
        {
            _workspaceMemberRepository = workspaceMemberRepository;
            _getWorkspaceMembersValidator = getWorkspaceMembersValidator;
            _paginationValidator = paginationValidator;
            _workspaceRepository = workspaceRepository;
            _changeWorkspaceMemberRoleValidator = changeWorkspaceMemberRoleValidator;
            _workspaceAuthorizationService = workspaceAuthorizationService;
            _currentUserService = currentUserService;
        }

        public async Task<PaginationResult<WorkspaceMember>> GetWorkspaceMembersAsync(Guid WorkspaceId, GetWorkspaceMembersRequest request, CancellationToken token)
        {
            await _paginationValidator.ValidateAndThrowAsync(request, token);
            await _getWorkspaceMembersValidator.ValidateAndThrowAsync(request, token);

            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(WorkspaceId, token);

            if (workspace is null)
            {
                throw new NotFoundException($"Workspace with ID {WorkspaceId} does not exist.");
            }

            return await _workspaceMemberRepository.GetWorkspaceMembersAsync(WorkspaceId ,request, token);
        }

        public async Task<bool> ChangeWorkspaceMemberRoleAsync(Guid workspaceId, Guid memberId,ChangeWorkspaceMemberRoleRequest request, CancellationToken token)
        {
            await _changeWorkspaceMemberRoleValidator.ValidateAndThrowAsync(request, token);

            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(workspaceId, token);

            if (workspace is null)
            {
                throw new NotFoundException($"Workspace with ID {workspaceId} does not exist.");
            }

            var canChangeRole = await _workspaceAuthorizationService.CanChangeRole(workspaceId, token);
            if (!canChangeRole)
            {
                throw new UnauthorizedException("User is not allowed to change role of the requested workspace member.");
            }

            var userId = _currentUserService.UserId;
            
            // user can not change his role.
            if (userId.Value == memberId) {
                throw new BadRequestException("User can not change his own role.");
            }
            
            var requestedMember = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(memberId, token);
            
            if(requestedMember is null){
                throw new NotFoundException("The requested member is not a member of the requested worksapce.");
            }

            if(requestedMember.Role == (WorkspaceRole)AllowedWorkspaceRole.Admin){
                throw new BadRequestException("Can not change owner's role.");
            }

            if(requestedMember.Role == (WorkspaceRole)request.Role){
                throw new BadRequestException("The requested role is the same as the requested member's current role.");
            }

            var currentUserMemberShip = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);

            if (currentUserMemberShip is null) {
                throw new NotFoundException("User is not a member of the requested worksapce.");
            }

            if (currentUserMemberShip.Role == WorkspaceRole.Admin) {

                if (!(requestedMember.Role == WorkspaceRole.Member || requestedMember.Role == WorkspaceRole.Guest))
                {
                    throw new UnauthorizedException("Admins can only change the role of a guest or member within the a workspace.");
                }
            }

            return await _workspaceMemberRepository.ChangeWorkspaceMemberRoleAsync(workspaceId, memberId, request, token);
        }

        public async Task<bool> RemoveWorkspaceMemberAsync(Guid workspaceId, Guid memberId, CancellationToken token)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(workspaceId, token);

            if (workspace is null)
            {
                throw new NotFoundException($"Workspace with ID {workspaceId} does not exist.");
            }

            var canRemoveMember = await _workspaceAuthorizationService.CanRemoveWorkspaceMember(workspaceId, token);
            if (!canRemoveMember)
            {
                throw new UnauthorizedException("User is not allowed to reomve the requested workspace member.");
            }

            var userId = _currentUserService.UserId;

            // user can not remove himself.
            if (userId.Value == memberId)
            {
                throw new BadRequestException("User cannot remove themselves from the workspace.");
            }

            var currentUserMemberShip = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);

            if (currentUserMemberShip is null) {
                throw new NotFoundException("User is not a member of the requested worksapce.");
            }

            var requestedMember = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(memberId, token);

            if (requestedMember is null)
            {
                throw new NotFoundException("The requested member is not a member of the requested worksapce.");
            }

            // can't remove workspace owner.
            if (requestedMember.Role == WorkspaceRole.Owner)
            {
                throw new BadRequestException("can not remove workspace's owner.");
            }


            if (currentUserMemberShip.Role == WorkspaceRole.Admin)
            {

                if (!(requestedMember.Role == WorkspaceRole.Member || requestedMember.Role == WorkspaceRole.Guest))
                {
                    throw new UnauthorizedException("Admins can only remove a guest or member within the a workspace.");
                }
            }

            return await _workspaceMemberRepository.RemoveWorkspaceMemberAsync(workspaceId, memberId, token);
        }

        public async Task<bool> LeaveWorkspaceAsync(Guid workspaceId, CancellationToken token)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(workspaceId, token);

            if (workspace is null)
            {
                throw new NotFoundException($"Workspace with ID {workspaceId} does not exist.");
            }

            var canLeaveWorkspace = await _workspaceAuthorizationService.CanLeaveWorkspace(workspaceId, token);
            if (!canLeaveWorkspace)
            {
                throw new BadRequestException("Owner can not leave a workspace until ownership is transfered.");
            }

            var userId = _currentUserService.UserId;

            var currentUserMemberShip = await _workspaceRepository.GetWorkspaceMembershipAsync(workspaceId, userId.Value, token);

            if (currentUserMemberShip is null)
            {
                throw new NotFoundException("User is not a member of the requested worksapce.");
            }

            return await _workspaceMemberRepository.LeaveWorkspaceAsync(workspaceId, userId.Value, token);
        }

        public async Task<WorkspaceMember?> GetWorkspaceMemberByIdAsync(Guid WorkspaceId, Guid memberId, CancellationToken token)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(WorkspaceId, token);

            if (workspace is null)
            {
                throw new NotFoundException($"Workspace with ID {WorkspaceId} does not exist.");
            }

            var member = await _workspaceMemberRepository.GetWorkspaceMemberByIdAsync(memberId, token);

            if(member is null) throw new NotFoundException($"Member with ID {memberId} does not exist within workspace with ID {WorkspaceId}.");

            return member;
        }
    }
}
