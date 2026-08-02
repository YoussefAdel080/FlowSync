using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class GetWorkspaceMembersValidator: AbstractValidator<GetWorkspaceMembersRequest>
    {
        public GetWorkspaceMembersValidator() {
            RuleFor(x => x.MemberName)
                .MaximumLength(100).WithMessage("MemberName must not exceed 100 characters.");

            RuleFor(x => x.JoinedAtFrom)
                .LessThanOrEqualTo(x => x.JoinedAtTo).WithMessage("JoinedAtFrom must be less than or equal to JoinedAtTo.");

            RuleFor(x => x.JoinedAtTo)
                .GreaterThanOrEqualTo(x => x.JoinedAtFrom).WithMessage("JoinedAtTo must be greater than or equal to JoinedAtFrom.");

            RuleFor(x => x.SortByValue)
                .IsInEnum().WithMessage("SortByValue must be a valid enum value.");
        }
    }
}
