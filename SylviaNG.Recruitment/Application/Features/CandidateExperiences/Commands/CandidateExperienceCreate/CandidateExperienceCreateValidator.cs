using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceCreate
{
    public class CandidateExperienceCreateValidator : AbstractValidator<CandidateExperienceCreateCommand>
    {
        public CandidateExperienceCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
