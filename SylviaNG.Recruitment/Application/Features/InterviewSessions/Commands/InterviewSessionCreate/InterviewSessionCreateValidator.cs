using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Commands.InterviewSessionCreate
{
    public class InterviewSessionCreateValidator : AbstractValidator<InterviewSessionCreateCommand>
    {
        public InterviewSessionCreateValidator()
        {
            RuleFor(x => x.Request.SessionTitle)
                .NotEmpty().WithMessage("SessionTitle is required.")
                .MaximumLength(500).WithMessage("SessionTitle must not exceed 500 characters.");

            RuleFor(x => x.Request.ScheduledDate)
                .GreaterThan(DateTime.MinValue).WithMessage("ScheduledDate is required.");

            RuleFor(x => x.Request.DurationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than 0.");
        }
    }
}
