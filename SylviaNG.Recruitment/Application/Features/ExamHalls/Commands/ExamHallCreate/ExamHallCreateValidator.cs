using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallCreate
{
    public class ExamHallCreateValidator : AbstractValidator<ExamHallCreateCommand>
    {
        public ExamHallCreateValidator()
        {
            RuleFor(x => x.Request.VenueName)
                .NotEmpty().WithMessage("VenueName is required.")
                .MaximumLength(500).WithMessage("VenueName must not exceed 500 characters.");
        }
    }
}
