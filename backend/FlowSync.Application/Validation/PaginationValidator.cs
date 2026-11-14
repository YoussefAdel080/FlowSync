using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class PaginationValidator: AbstractValidator<PaginationRequest>
    {
        public PaginationValidator() {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.");

            RuleFor(x => x.Sort)
                .IsInEnum().WithMessage("Sort must be a valid enum value.");
        }
    }
}
