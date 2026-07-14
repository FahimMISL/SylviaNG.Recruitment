using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterPreview
{
    public class ShortlistFilterPreviewValidator : AbstractValidator<ShortlistFilterPreviewQuery>
    {
        public ShortlistFilterPreviewValidator()
        {
            RuleFor(x => x.Request.JobPostingId)
                .GreaterThan(0).WithMessage("A job posting is required to preview a filter.");

            RuleFor(x => x.Request)
                .Must(r => r.ShortlistFilterId.HasValue ^ (r.Definition != null))
                .WithMessage("Provide either a saved ShortlistFilterId or an unsaved Definition, not both or neither.");
        }
    }
}
