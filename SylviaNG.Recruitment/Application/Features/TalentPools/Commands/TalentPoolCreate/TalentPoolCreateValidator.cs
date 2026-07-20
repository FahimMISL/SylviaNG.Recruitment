using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCreate
{
    public class TalentPoolCreateValidator : AbstractValidator<TalentPoolCreateCommand>
    {
        public TalentPoolCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");
        }
    }
}
