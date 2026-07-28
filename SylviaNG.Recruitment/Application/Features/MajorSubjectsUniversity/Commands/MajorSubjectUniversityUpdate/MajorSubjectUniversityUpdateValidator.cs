using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityUpdate
{
    public class MajorSubjectUniversityUpdateValidator : AbstractValidator<MajorSubjectUniversityUpdateCommand>
    {
        public MajorSubjectUniversityUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");
        }
    }
}
