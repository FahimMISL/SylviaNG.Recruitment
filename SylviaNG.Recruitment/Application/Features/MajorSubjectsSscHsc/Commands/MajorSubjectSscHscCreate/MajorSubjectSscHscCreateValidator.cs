using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscCreate
{
    public class MajorSubjectSscHscCreateValidator : AbstractValidator<MajorSubjectSscHscCreateCommand>
    {
        public MajorSubjectSscHscCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
