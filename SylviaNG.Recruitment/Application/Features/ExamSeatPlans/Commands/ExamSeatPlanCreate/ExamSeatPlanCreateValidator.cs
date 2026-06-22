using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanCreate
{
    public class ExamSeatPlanCreateValidator : AbstractValidator<ExamSeatPlanCreateCommand>
    {
        public ExamSeatPlanCreateValidator()
        {
            RuleFor(x => x.Request.ExamId).GreaterThan(0).WithMessage("ExamId is required.");
        }
    }
}
