using FluentValidation;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamCreate
{
    public class ExamCreateValidator : AbstractValidator<ExamCreateCommand>
    {
        public ExamCreateValidator()
        {
            RuleFor(x => x.Request.JobPostingId)
                .GreaterThan(0).WithMessage("JobPostingId is required.");

            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200);

            RuleFor(x => x.Request.ScheduledStartAt)
                .NotEqual(default(DateTime)).WithMessage("ScheduledStartAt is required.");

            RuleFor(x => x.Request.DurationMinutes)
                .GreaterThan(0).WithMessage("DurationMinutes must be greater than 0.");

            RuleFor(x => x.Request.TotalMarks)
                .GreaterThan(0).WithMessage("TotalMarks must be greater than 0.");

            RuleFor(x => x.Request.PassMarks)
                .GreaterThan(0).WithMessage("PassMarks must be greater than 0.")
                .LessThanOrEqualTo(x => x.Request.TotalMarks).WithMessage("PassMarks cannot exceed TotalMarks.");

            RuleFor(x => x.Request.ExamVenueId)
                .NotNull().WithMessage("ExamVenueId is required for an in-person exam.")
                .When(x => x.Request.ExamType == ExamTypeEnum.InPerson);

            RuleFor(x => x.Request.QuestionGroupId)
                .NotNull().WithMessage("QuestionGroupId is required for an online exam.")
                .When(x => x.Request.ExamType == ExamTypeEnum.Online);
        }
    }
}
