using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaCreate
{
    public class ShortlistFilterCriteriaCreateValidator : AbstractValidator<ShortlistFilterCriteriaCreateCommand>
    {
        public ShortlistFilterCriteriaCreateValidator()
        {
            RuleFor(x => x.Request.ShortlistFilterId).GreaterThan(0).WithMessage("ShortlistFilterId is required.");
        }
    }
}
