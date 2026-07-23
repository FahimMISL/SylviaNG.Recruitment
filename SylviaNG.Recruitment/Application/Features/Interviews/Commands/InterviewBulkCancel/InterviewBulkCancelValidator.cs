using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewBulkCancel
{
    public class InterviewBulkCancelValidator : AbstractValidator<InterviewBulkCancelCommand>
    {
        public InterviewBulkCancelValidator()
        {
            RuleFor(x => x.Request.InterviewIds)
                .NotEmpty().WithMessage("At least one InterviewId is required.");

            RuleFor(x => x.Request.CancellationReason)
                .NotEmpty().WithMessage("CancellationReason is required.")
                .MaximumLength(500).WithMessage("CancellationReason must not exceed 500 characters.");
        }
    }
}
