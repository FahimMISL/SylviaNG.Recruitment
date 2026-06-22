using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordCreate
{
    public class OnboardingRecordCreateValidator : AbstractValidator<OnboardingRecordCreateCommand>
    {
        public OnboardingRecordCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
