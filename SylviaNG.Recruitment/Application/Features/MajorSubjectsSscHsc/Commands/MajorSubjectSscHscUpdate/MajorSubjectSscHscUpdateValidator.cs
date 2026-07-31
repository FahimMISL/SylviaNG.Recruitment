using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscUpdate
{
    public class MajorSubjectSscHscUpdateValidator : AbstractValidator<MajorSubjectSscHscUpdateCommand>
    {
        public MajorSubjectSscHscUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
