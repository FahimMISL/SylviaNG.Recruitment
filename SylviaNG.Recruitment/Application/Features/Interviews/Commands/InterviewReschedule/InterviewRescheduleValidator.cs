using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewReschedule
{
    public class InterviewRescheduleValidator : AbstractValidator<InterviewRescheduleCommand>
    {
        public InterviewRescheduleValidator()
        {
            RuleFor(x => x.Request.ScheduledEndAt)
                .GreaterThan(x => x.Request.ScheduledStartAt).WithMessage("ScheduledEndAt must be after ScheduledStartAt.");
        }
    }
}
