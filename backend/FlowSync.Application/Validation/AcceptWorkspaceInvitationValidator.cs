using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class AcceptWorkspaceInvitationValidator: AbstractValidator<AcceptWorkspaceInvitationRequest>
    {
        public AcceptWorkspaceInvitationValidator() {
            RuleFor(x => x.InvitationToken)
                .NotEmpty().WithMessage("Invitation token is required.");
        }
    }
}
