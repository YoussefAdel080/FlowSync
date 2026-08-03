using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class RemoveWorkspaceMemberValidator: AbstractValidator<RemoveWorkspaceMemberRequest>
    {
        public RemoveWorkspaceMemberValidator() {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("MemberId is required.");
        }
    }
}
