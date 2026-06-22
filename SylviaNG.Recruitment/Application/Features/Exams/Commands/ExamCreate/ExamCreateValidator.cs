using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamCreate
{
    public class ExamCreateValidator : AbstractValidator<ExamCreateCommand>
    {
        public ExamCreateValidator()
        {
            RuleFor(x => x.Request.RequisitionId)
                .GreaterThan(0).WithMessage("RequisitionId is required.");

            RuleFor(x => x.Request.ExamTitle)
                .NotEmpty().WithMessage("ExamTitle is required.")
                .MaximumLength(500).WithMessage("ExamTitle must not exceed 500 characters.");
        }
    }
}
