using FlowSync.Contracts.Enums;
using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class InviteToWorkspaceValidator: AbstractValidator<InviteToWorkspaceRequest>
    {
        public InviteToWorkspaceValidator() {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.WorkspaceId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Workspace ID is required.");

            RuleFor(x => x.Role)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Role is required.")
                .Must(role => role != WorkspaceRole.Owner)
                .WithMessage("You cannot invite a user as the workspace owner."); ;
        }
    }
}
