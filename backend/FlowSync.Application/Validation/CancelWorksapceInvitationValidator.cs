using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class CancelWorksapceInvitationValidator: AbstractValidator<CancelWorkspaceInvitationRequest>
    {
        public CancelWorksapceInvitationValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Invitation ID is required.");
        }
    }
}
