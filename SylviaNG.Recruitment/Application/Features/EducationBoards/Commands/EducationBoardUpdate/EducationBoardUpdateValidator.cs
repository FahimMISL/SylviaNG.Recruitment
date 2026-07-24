using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardUpdate
{
    public class EducationBoardUpdateValidator : AbstractValidator<EducationBoardUpdateCommand>
    {
        public EducationBoardUpdateValidator()
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
