using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewCancel
{
    public class InterviewCancelValidator : AbstractValidator<InterviewCancelCommand>
    {
        public InterviewCancelValidator()
        {
            RuleFor(x => x.Request.CancellationReason)
                .NotEmpty().WithMessage("CancellationReason is required.")
                .MaximumLength(500).WithMessage("CancellationReason must not exceed 500 characters.");
        }
    }
}
