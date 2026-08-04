using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class ChangeWorkspaceMemberRoleValidator: AbstractValidator<ChangeWorkspaceMemberRoleRequest>
    {
        public ChangeWorkspaceMemberRoleValidator() {
            RuleFor(x => x.Role)
                .IsInEnum();
        }
    }
}
