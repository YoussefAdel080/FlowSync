using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class DeleteWorkspaceValidator: AbstractValidator<DeleteWorkspaceRequest>
    {
        public DeleteWorkspaceValidator() {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Workspace id is required.");
        }
    }
}
