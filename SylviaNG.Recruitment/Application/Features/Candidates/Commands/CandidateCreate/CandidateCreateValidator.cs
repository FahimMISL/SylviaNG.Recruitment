using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateCreate
{
    public class CandidateCreateValidator : AbstractValidator<CandidateCreateCommand>
    {
        public CandidateCreateValidator()
        {
            RuleFor(x => x.Request.FullName).NotEmpty().WithMessage("Full name is required.").MaximumLength(200);
            RuleFor(x => x.Request.Email).NotEmpty().WithMessage("Email is required.").EmailAddress();
        }
    }
}
