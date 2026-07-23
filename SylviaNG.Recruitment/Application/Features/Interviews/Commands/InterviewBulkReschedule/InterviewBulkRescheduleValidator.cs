using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkReschedule
{
    public class InterviewBulkRescheduleValidator : AbstractValidator<InterviewBulkRescheduleCommand>
    {
        public InterviewBulkRescheduleValidator()
        {
            RuleFor(x => x.Request.InterviewIds)
                .NotEmpty().WithMessage("At least one InterviewId is required.");

            RuleFor(x => x.Request.GapMinutes)
                .GreaterThanOrEqualTo(0).WithMessage("GapMinutes cannot be negative.");
        }
    }
}
