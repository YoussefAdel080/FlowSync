using FlowSync.Application.Common.Pagination;
using FlowSync.Application.Exceptions;
using FlowSync.Application.Models;
using FlowSync.Application.Repositories;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Services
{
    public class WorkspaceMemberService : IWorkspaceMemberService
    {
        private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IValidator<GetWorkspaceMembersRequest> _getWorkspaceMembersValidator;
        private readonly IValidator<PaginationRequest> _paginationValidator;

        public WorkspaceMemberService(
            IWorkspaceMemberRepository workspaceMemberRepository,
            IValidator<GetWorkspaceMembersRequest> getWorkspaceMembersValidator,
            IValidator<PaginationRequest> paginationValidator,
            IWorkspaceRepository workspaceRepository)
        {
            _workspaceMemberRepository = workspaceMemberRepository;
            _getWorkspaceMembersValidator = getWorkspaceMembersValidator;
            _paginationValidator = paginationValidator;
            _workspaceRepository = workspaceRepository;
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
    }
}
