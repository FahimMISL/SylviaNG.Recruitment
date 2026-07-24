using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityCreate
{
    public class MajorSubjectUniversityCreateValidator : AbstractValidator<MajorSubjectUniversityCreateCommand>
    {
        public MajorSubjectUniversityCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");
        }
    }
}
