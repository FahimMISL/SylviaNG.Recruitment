using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Commands.CandidateEducationCreate
{
    public class CandidateEducationCreateValidator : AbstractValidator<CandidateEducationCreateCommand>
    {
        public CandidateEducationCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
