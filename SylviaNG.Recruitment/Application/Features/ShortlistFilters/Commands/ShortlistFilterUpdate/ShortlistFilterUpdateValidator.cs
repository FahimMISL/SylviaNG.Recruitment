using FluentValidation;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterUpdate
{
    public class ShortlistFilterUpdateValidator : AbstractValidator<ShortlistFilterUpdateCommand>
    {
        public ShortlistFilterUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Filter name is required.")
                .MaximumLength(200).WithMessage("Filter name must not exceed 200 characters.");

            RuleFor(x => x.Request.Criteria)
                .NotEmpty().WithMessage("A shortlist filter must have at least one criterion.");

            RuleForEach(x => x.Request.Criteria).ChildRules(criterion =>
            {
                criterion.RuleFor(c => c.MinEducationLevel)
                    .NotNull().When(c => c.CriterionType == CriterionTypeEnum.EducationLevel)
                    .WithMessage("Minimum education level is required for an Education Level criterion.");

                criterion.RuleFor(c => c.MinExperienceYears)
                    .NotNull().When(c => c.CriterionType == CriterionTypeEnum.MinExperienceYears)
                    .WithMessage("Minimum experience years is required for a Minimum Experience Years criterion.")
                    .GreaterThanOrEqualTo(0).When(c => c.MinExperienceYears.HasValue)
                    .WithMessage("Minimum experience years must be greater than or equal to 0.");

                criterion.RuleFor(c => c.RequiredSkillNames)
                    .NotEmpty().When(c => c.CriterionType == CriterionTypeEnum.RequiredSkills)
                    .WithMessage("At least one required skill is required for a Required Skills criterion.");

                criterion.RuleFor(c => c)
                    .Must(c => c.MinAge.HasValue || c.MaxAge.HasValue)
                    .When(c => c.CriterionType == CriterionTypeEnum.AgeRange)
                    .WithMessage("At least one of minimum or maximum age is required for an Age Range criterion.");

                criterion.RuleFor(c => c.RequiredDistrict)
                    .NotEmpty().When(c => c.CriterionType == CriterionTypeEnum.District)
                    .WithMessage("A district is required for a District criterion.");

                criterion.RuleFor(c => c.MinScreeningScore)
                    .NotNull().When(c => c.CriterionType == CriterionTypeEnum.MinScreeningScore)
                    .WithMessage("Minimum screening score is required for a Minimum Screening Score criterion.");
            });
        }
    }
}
