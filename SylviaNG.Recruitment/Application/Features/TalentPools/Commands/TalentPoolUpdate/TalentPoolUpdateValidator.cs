using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolUpdate
{
    public class TalentPoolUpdateValidator : AbstractValidator<TalentPoolUpdateCommand>
    {
        public TalentPoolUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");
        }
    }
}
