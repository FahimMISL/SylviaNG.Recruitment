using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolFastTrack
{
    public class TalentPoolFastTrackValidator : AbstractValidator<TalentPoolFastTrackCommand>
    {
        public TalentPoolFastTrackValidator()
        {
            RuleFor(x => x.Request.CandidateProfileIds)
                .NotEmpty().WithMessage("At least one CandidateProfileId is required.");

            RuleFor(x => x.Request.JobPostingId)
                .GreaterThan(0).WithMessage("JobPostingId is required.");
        }
    }
}
