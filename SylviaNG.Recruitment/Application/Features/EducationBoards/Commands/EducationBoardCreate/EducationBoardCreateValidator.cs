using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardCreate
{
    public class EducationBoardCreateValidator : AbstractValidator<EducationBoardCreateCommand>
    {
        public EducationBoardCreateValidator()
        {
            RuleFor(x => x.Request.Code)
                .NotEmpty().WithMessage("Board code is required.")
                .MaximumLength(10).WithMessage("Board code must not exceed 10 characters.");

            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Board name is required.")
                .MaximumLength(150).WithMessage("Board name must not exceed 150 characters.");
        }
    }
}
