using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class ChangeWorkspaceMemberRoleValidator: AbstractValidator<ChangeWorkspaceMemberRoleRequest>
    {
        public ChangeWorkspaceMemberRoleValidator() {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("MemeberId is required.");

            RuleFor(x => x.Role)
                .IsInEnum();
        }
    }
}
