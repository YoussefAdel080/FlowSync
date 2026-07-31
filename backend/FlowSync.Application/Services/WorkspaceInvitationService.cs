using FlowSync.Application.Exceptions;
using FlowSync.Application.Repositories;
using FlowSync.Application.Validation;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Services
{
    public class WorkspaceInvitationService: IWorkspaceInvitationService
    {
        private readonly IWorkspaceInvitationRepository _workspaceInvitationRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        private readonly IValidator<InviteToWorkspaceRequest> _inviteToWorkspaceValidator;
        private readonly IValidator<AcceptWorkspaceInvitationRequest> _acceptWorkspaceInvitationValidator;
        private readonly IValidator<DeclineWorkspaceInvitationRequest> _declineWorkspaceInvitationValidator;
        private readonly IValidator<CancelWorkspaceInvitationRequest> _cancelWorkspaceInvitationValidator;
        public WorkspaceInvitationService(IWorkspaceInvitationRepository workspaceInvitationRepository, IAuthRepository authRepository, InviteToWorkspaceValidator inviteToWorkspaceValidator, IWorkspaceRepository workspaceRepository, IWorkspaceAuthorizationService workspaceAuthorizationService ,IValidator<AcceptWorkspaceInvitationRequest> acceptWorkspaceInvitationValidator, IValidator<DeclineWorkspaceInvitationRequest> declineWorkspaceInvitationValidator, IValidator<CancelWorkspaceInvitationRequest> cancelWorkspaceInvitationValidator)
        {
            _workspaceInvitationRepository = workspaceInvitationRepository;
            _authRepository = authRepository;
            _inviteToWorkspaceValidator = inviteToWorkspaceValidator;
            _workspaceRepository = workspaceRepository;
            _workspaceAuthorizationService = workspaceAuthorizationService;
            _acceptWorkspaceInvitationValidator = acceptWorkspaceInvitationValidator;
            _declineWorkspaceInvitationValidator = declineWorkspaceInvitationValidator;
            _cancelWorkspaceInvitationValidator = cancelWorkspaceInvitationValidator;
        }

        public async Task<bool> CreateWorkspaceInvitationAsync(InviteToWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            await _inviteToWorkspaceValidator.ValidateAndThrowAsync(request, token);

            var canInvite = await _workspaceAuthorizationService.CanInvite(request.WorkspaceId, token);

            if (!canInvite)
            {
                throw new UnauthorizedException("User is not allowed to invite users to the requested workspace.");
            }

            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(request.WorkspaceId, token);

            if (workspace is null)
            {
                throw new NotFoundException($"Workspace with ID {request.WorkspaceId} does not exist.");
            }

            // user cannot invite himself to the workspace
            var user = await _authRepository.GetUserByIdAsync(userId, token);

            if (user is not null && user.Email != request.Email)
            {
                throw new BadRequestException("User is not allowed to invite himself.");
            }

            // user cannot invite someone who is already a member of the workspace
            var existingMembership = await _workspaceRepository.IsWorkspaceMemberByEmailAsync(request.Email, request.WorkspaceId, token);

            if (existingMembership)
            {
                throw new BadRequestException("User is already a member of the workspace.");
            }

            // user cannot invite someone who has pending invitation to the workspace
            var hasPendingInvitation = await _workspaceInvitationRepository.InvitationExistsByEmailAsync(request.Email, request.WorkspaceId, token);

            if (hasPendingInvitation)
            {
                throw new BadRequestException("A pending invitation exists for the requested email.");
            }

            return await _workspaceInvitationRepository.CreateWorkspaceInvitationAsync(request, userId, token);
        }

        public async Task<bool> AcceptWorkspaceInvitationAsync(AcceptWorkspaceInvitationRequest request, Guid userId, CancellationToken token)
        {
            await _acceptWorkspaceInvitationValidator.ValidateAndThrowAsync(request, token);

            var user = await _authRepository.GetUserByIdAsync(userId, token);

            if (user is null)
            {
                throw new NotFoundException($"User with ID {userId} does not exist.");
            }

            var invitation = await _workspaceInvitationRepository.GetWorkspaceInvitationByTokenAndEmailAsync(request.InvitationToken, user.Email, token);

            if(invitation is null)
            {
                throw new NotFoundException($"Invitation with token {request.InvitationToken} does not exist.");
            }

            if(invitation.Status != Enums.WorkspaceInvitationStatus.Pending)
            {
                throw new BadRequestException($"Invitation with token {request.InvitationToken} is expired or got accept/declined.");
            }

            return await _workspaceInvitationRepository.AcceptWorkspaceInvitationAsync(request, user.Email, userId, token);
        }

        public async Task<bool> DeclineWorkspaceInvitationAsync(DeclineWorkspaceInvitationRequest request, Guid userId, CancellationToken token)
        {
            await _declineWorkspaceInvitationValidator.ValidateAndThrowAsync(request, token);

            var user = await _authRepository.GetUserByIdAsync(userId, token);

            if (user is null)
            {
                throw new NotFoundException($"User with ID {userId} does not exist.");
            }

            var invitation = await _workspaceInvitationRepository.GetWorkspaceInvitationByTokenAndEmailAsync(request.InvitationToken, user.Email, token);

            if (invitation is null)
            {
                throw new NotFoundException($"Invitation with token {request.InvitationToken} does not exist.");
            }

            if (invitation.Status != Enums.WorkspaceInvitationStatus.Pending)
            {
                throw new BadRequestException($"Invitation with token {request.InvitationToken} is expired or got accepted/declined.");
            }

            return await _workspaceInvitationRepository.DeclineWorkspaceInvitationAsync(request, user.Email, userId, token);
        }

        public async Task<bool> CancelWorkspaceInvitationAsync(CancelWorkspaceInvitationRequest request, Guid userId, CancellationToken token)
        {
            await _cancelWorkspaceInvitationValidator.ValidateAndThrowAsync(request, token);

            var invitation = await _workspaceInvitationRepository.GetWorkspaceInvitationByIdAsync(request.Id, token);

            if (invitation is null)
            {
                throw new NotFoundException($"Invitation with id {request.Id} does not exist.");
            }

            var user = await _authRepository.GetUserByIdAsync(userId, token);

            if (user is null)
            {
                throw new NotFoundException($"User with ID {userId} does not exist.");
            }

            var canInvite = await _workspaceAuthorizationService.CanCancelInvitation(invitation.WorkspaceId ,token);

            if (!canInvite)
            {
                throw new UnauthorizedException("User is not allowed to invite users to the requested workspace.");
            }

            if (invitation.Status != Enums.WorkspaceInvitationStatus.Pending)
            {
                throw new BadRequestException($"Invitation is expired or got accepted/declined.");
            }
                
            return await _workspaceInvitationRepository.CancelWorkspaceInvitationAsync(request, token);
        }
    }
}
