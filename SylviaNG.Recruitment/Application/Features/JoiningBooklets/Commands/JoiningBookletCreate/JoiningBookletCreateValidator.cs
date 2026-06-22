using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletCreate
{
    public class JoiningBookletCreateValidator : AbstractValidator<JoiningBookletCreateCommand>
    {
        public JoiningBookletCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId)
                .GreaterThan(0).WithMessage("CandidateId is required.");

            RuleFor(x => x.Request.FileUrl)
                .NotEmpty().WithMessage("FileUrl is required.")
                .MaximumLength(500).WithMessage("FileUrl must not exceed 500 characters.");
        }
    }
}
