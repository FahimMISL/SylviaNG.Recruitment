using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterApply
{
    public class ShortlistFilterApplyValidator : AbstractValidator<ShortlistFilterApplyCommand>
    {
        public ShortlistFilterApplyValidator()
        {
            RuleFor(x => x.Request.ShortlistFilterId)
                .GreaterThan(0).WithMessage("A saved shortlist filter is required.");

            RuleFor(x => x.Request.JobPostingId)
                .GreaterThan(0).WithMessage("A job posting is required to apply a filter.");
        }
    }
}
