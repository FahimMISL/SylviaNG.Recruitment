using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallCreate
{
    public class ExamHallCreateValidator : AbstractValidator<ExamHallCreateCommand>
    {
        public ExamHallCreateValidator()
        {
            RuleFor(x => x.Request.HallName)
                .NotEmpty().WithMessage("Hall name is required.")
                .MaximumLength(200).WithMessage("Hall name must not exceed 200 characters.");

            RuleFor(x => x.Request.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(300).WithMessage("Location must not exceed 300 characters.");

            RuleFor(x => x.Request.TotalCapacity)
                .GreaterThan(0).WithMessage("Total capacity must be greater than zero.");
        }
    }
}
