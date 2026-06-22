using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailCreate
{
    public class InsuranceDetailCreateValidator : AbstractValidator<InsuranceDetailCreateCommand>
    {
        public InsuranceDetailCreateValidator()
        {
            RuleFor(x => x.Request.PreBoardingProfileId).GreaterThan(0).WithMessage("PreBoardingProfileId is required.");
        }
    }
}
