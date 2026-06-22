using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultCreate
{
    public class ApplicationScreeningResultCreateValidator : AbstractValidator<ApplicationScreeningResultCreateCommand>
    {
        public ApplicationScreeningResultCreateValidator()
        {
            RuleFor(x => x.Request.JobApplicationId).GreaterThan(0).WithMessage("JobApplicationId is required.");
        }
    }
}
