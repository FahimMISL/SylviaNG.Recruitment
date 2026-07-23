using FluentValidation;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkSchedule
{
    public class InterviewBulkScheduleValidator : AbstractValidator<InterviewBulkScheduleCommand>
    {
        public InterviewBulkScheduleValidator()
        {
            RuleFor(x => x.Request.JobApplicationIds)
                .NotEmpty().WithMessage("At least one JobApplicationId is required.");

            RuleFor(x => x.Request.DurationMinutes)
                .GreaterThan(0).WithMessage("DurationMinutes must be greater than zero.");

            RuleFor(x => x.Request.GapMinutes)
                .GreaterThanOrEqualTo(0).WithMessage("GapMinutes cannot be negative.");

            RuleFor(x => x.Request.InterviewRoomId)
                .NotNull().WithMessage("InterviewRoomId is required for an in-person interview.")
                .When(x => x.Request.InterviewType == InterviewTypeEnum.InPerson);

            RuleFor(x => x.Request.MeetingLink)
                .NotEmpty().WithMessage("MeetingLink is required for a virtual interview.")
                .When(x => x.Request.InterviewType == InterviewTypeEnum.Virtual);

            RuleFor(x => x.Request.Round)
                .GreaterThan(0).WithMessage("Round must be greater than zero.");
        }
    }
}
