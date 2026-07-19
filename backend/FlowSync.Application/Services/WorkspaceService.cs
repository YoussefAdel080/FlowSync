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
        private readonly IValidator<CreateWorkspaceRequest> _createWorkspaceValidator;
        private readonly IValidator<UpdateWorkspaceRequest> _updateWorkspaceValidator;

        public WorkspaceService(IWorkspaceRepository workspaceRepository, IValidator<CreateWorkspaceRequest> createWorkspaceValidator, IValidator<UpdateWorkspaceRequest> updateWorkspaceValidator)
        {
            _workspaceRepository = workspaceRepository;
            _createWorkspaceValidator = createWorkspaceValidator;
            _updateWorkspaceValidator = updateWorkspaceValidator;
        }

        public async Task<bool> CreateWorkspaceAsync(CreateWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            await _createWorkspaceValidator.ValidateAndThrowAsync(request, token);

            return await _workspaceRepository.CreateWorkspaceAsync(request, userId, token);
        }

        public async Task<bool> UpdateWorkspaceAsync(UpdateWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            var workspaceExists = await _workspaceRepository.WorkspaceExistsByIdAsync(request.Id, token);

            if(!workspaceExists)
            {
                throw new NotFoundException($"Workspace with ID {request.Id} does not exist.");
            }

            var isWorkspaceOwner = await _workspaceRepository.IsWorkspaceOwnerAsync(request.Id, userId, token);

            if(!isWorkspaceOwner)
            {
                throw new ForbiddenException($"You are not allowed to update this workspace.");
            }

            await _updateWorkspaceValidator.ValidateAndThrowAsync(request, token);
            
            return await _workspaceRepository.UpdateWorkspaceAsync(request, userId, token);
        }

        public async Task<bool> DeleteWorkspaceAsync(DeleteWorkspaceRequest request, Guid userId, CancellationToken token)
        {
            var workspaceExists = await _workspaceRepository.WorkspaceExistsByIdAsync(request.Id, token);

            if (!workspaceExists)
            {
                throw new NotFoundException($"Workspace with ID {request.Id} does not exist.");
            }

            var isWorkspaceOwner = await _workspaceRepository.IsWorkspaceOwnerAsync(request.Id, userId, token);

            if (!isWorkspaceOwner)
            {
                throw new ForbiddenException($"You are not allowed to update this workspace.");
            }

            return await _workspaceRepository.DeleteWorkspaceAsync(request, userId, token);
        }

        public async Task<IEnumerable<Workspace>> GetMyWorkspacesAsync(Guid userId, CancellationToken token)
        {
            return await _workspaceRepository.GetMyWorkspacesAsync(userId, token);
        }

        public async Task<Workspace> GetWorkspaceByIdAsync(Guid id, Guid userId, CancellationToken token)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(id, token);

            if (workspace is null)
            {
                throw new NotFoundException($"Workspace with ID {id} does not exist.");
            }

            var isWorkspaceOwner = await _workspaceRepository.IsWorkspaceOwnerAsync(id, userId, token);

            if (!isWorkspaceOwner)
            {
                throw new ForbiddenException($"You are not allowed to update this workspace.");
            }

            return workspace;
        }
    }
}
