using FlowSync.Contracts.Requests;
using FluentValidation;

namespace FlowSync.Application.Validation
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name Is Required.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name Is Required.");
        }
    }
}
