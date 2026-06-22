using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelCreate
{
    public class JobPostingChannelCreateValidator : AbstractValidator<JobPostingChannelCreateCommand>
    {
        public JobPostingChannelCreateValidator()
        {
            RuleFor(x => x.Request.JobPostingId).GreaterThan(0).WithMessage("JobPostingId is required.");
        }
    }
}
