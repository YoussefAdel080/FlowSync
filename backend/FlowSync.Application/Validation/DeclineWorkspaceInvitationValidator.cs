using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class DeclineWorkspaceInvitationValidator: AbstractValidator<DeclineWorkspaceInvitationRequest>
    {
        public DeclineWorkspaceInvitationValidator() {
            RuleFor(x => x.InvitationToken)
                .NotEmpty().WithMessage("Invitation token is required.");
        }
    }
}
