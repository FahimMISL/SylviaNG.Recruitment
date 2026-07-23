using FluentValidation;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewSchedule
{
    public class InterviewScheduleValidator : AbstractValidator<InterviewScheduleCommand>
    {
        public InterviewScheduleValidator()
        {
            RuleFor(x => x.Request.JobApplicationId)
                .GreaterThan(0).WithMessage("JobApplicationId is required.");

            RuleFor(x => x.Request.ScheduledEndAt)
                .GreaterThan(x => x.Request.ScheduledStartAt).WithMessage("ScheduledEndAt must be after ScheduledStartAt.");

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
