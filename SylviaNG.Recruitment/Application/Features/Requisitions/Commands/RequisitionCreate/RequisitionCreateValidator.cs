using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionCreate
{
    public class RequisitionCreateValidator : AbstractValidator<RequisitionCreateCommand>
    {
        public RequisitionCreateValidator()
        {
            RuleFor(x => x.Request.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(300);
        }
    }
}
