using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class GetPendingWorkspaceInvitationsValidator : AbstractValidator<GetPendingWorkspaceInvitationsRequest>
    {
        public GetPendingWorkspaceInvitationsValidator()
        {
            RuleFor(x => x.WorkspaceId)
                .NotEmpty().WithMessage("WorkspaceId is required.");
        }
    }
}
