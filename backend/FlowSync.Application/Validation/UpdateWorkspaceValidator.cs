using FlowSync.Application.Repositories;
using FlowSync.Application.Services;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class UpdateWorkspaceValidator: AbstractValidator<UpdateWorkspaceRequest>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly ICurrentUserService _currentUserService;
        public UpdateWorkspaceValidator(IWorkspaceRepository workspaceRepository, ICurrentUserService currentUserService)
        {
            _workspaceRepository = workspaceRepository;
            _currentUserService = currentUserService;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Workspace name is required.")
                .MaximumLength(80).WithMessage("Workspace name must not exceed 80 characters.")
                .MustAsync(async (name, cancellationToken) =>
                {
                    var userId = _currentUserService.UserId;
                    if (userId == null)
                    {
                        return false;
                    }
                    var workspaceExists = await _workspaceRepository.WorkspaceExistsByNameAsync(name.Trim().ToUpperInvariant(), userId.Value, cancellationToken);
                    return !workspaceExists;
                });
        }
    }
}
