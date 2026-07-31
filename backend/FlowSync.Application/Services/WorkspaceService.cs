using FlowSync.Application.Exceptions;
using FlowSync.Application.Models;
using FlowSync.Application.Repositories;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Services
{
    public class WorkspaceService: IWorkspaceService
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IWorkspaceAuthorizationService _workspaceAuthorizationService;
        private readonly IValidator<CreateWorkspaceRequest> _createWorkspaceValidator;
        private readonly IValidator<UpdateWorkspaceRequest> _updateWorkspaceValidator;
        private readonly IValidator<DeleteWorkspaceRequest> _deleteWorkspaceValidator;

        public WorkspaceService(IWorkspaceRepository workspaceRepository, IValidator<CreateWorkspaceRequest> createWorkspaceValidator, IValidator<UpdateWorkspaceRequest> updateWorkspaceValidator, IWorkspaceAuthorizationService workspaceAuthorizationService, IValidator<DeleteWorkspaceRequest> deleteWorkspaceValidator)
        {
            _workspaceRepository = workspaceRepository;
            _workspaceAuthorizationService = workspaceAuthorizationService;
            _createWorkspaceValidator = createWorkspaceValidator;
            _updateWorkspaceValidator = updateWorkspaceValidator;
            _deleteWorkspaceValidator = deleteWorkspaceValidator;
        }

        public async Task<bool> CreateWorkspaceAsync(CreateWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            await _createWorkspaceValidator.ValidateAndThrowAsync(request, token);

            return await _workspaceRepository.CreateWorkspaceAsync(request, userId, token);
        }

        public async Task<bool> UpdateWorkspaceAsync(UpdateWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            await _updateWorkspaceValidator.ValidateAndThrowAsync(request, token);

            var canUpdate = await _workspaceAuthorizationService.CanUpdate(request.Id, token);

            if (!canUpdate)
            {
                throw new UnauthorizedException("User is not allowed to update the requested workspace.");
            }

            var workspaceExists = await _workspaceRepository.WorkspaceExistsByIdAsync(request.Id, token);

            if(!workspaceExists)
            {
                throw new NotFoundException($"Workspace with ID {request.Id} does not exist.");
            }
            
            return await _workspaceRepository.UpdateWorkspaceAsync(request, userId, token);
        }

        public async Task<bool> DeleteWorkspaceAsync(DeleteWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            await _deleteWorkspaceValidator.ValidateAndThrowAsync(request, token);

            var canDelete = await _workspaceAuthorizationService.CanDelete(request.Id, token);

            if (!canDelete)
            {
                throw new UnauthorizedException("User is not allowed to delete the requested workspace.");
            }

            var workspaceExists = await _workspaceRepository.WorkspaceExistsByIdAsync(request.Id, token);

            if (!workspaceExists)
            {
                throw new NotFoundException($"Workspace with ID {request.Id} does not exist.");
            }

            return await _workspaceRepository.DeleteWorkspaceAsync(request, userId, token);
        }

        public async Task<IEnumerable<Workspace>> GetMyWorkspacesAsync(Guid userId, CancellationToken token)
        {
            return await _workspaceRepository.GetMyWorkspacesAsync(userId, token);
        }

        public async Task<Workspace?> GetWorkspaceByIdAsync(Guid id, Guid userId, CancellationToken token)
        {
            var canView = await _workspaceAuthorizationService.CanView(token);

            if (!canView)
            {
                throw new UnauthorizedException("User is not allowed to view the requested workspace."); ;
            }

            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(id, token);

            if (workspace is null)
            {
                throw new NotFoundException($"Workspace with ID {id} does not exist.");
            }

            return workspace;
        }
    }
}
