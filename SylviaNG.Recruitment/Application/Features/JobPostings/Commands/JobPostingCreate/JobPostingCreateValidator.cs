using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobPostingCreate
{
    public class JobPostingCreateValidator : AbstractValidator<JobPostingCreateCommand>
    {
        public JobPostingCreateValidator()
        {
            RuleFor(x => x.Request.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Request.SiteId)
                .GreaterThan(0).WithMessage("SiteId is required.");

            RuleFor(x => x.Request.HiringPipelineId)
                .GreaterThan(0).WithMessage("A hiring pipeline must be selected for every job opening.");

            RuleFor(x => x.Request.NumberOfPositions)
                .GreaterThan(0).WithMessage("Number of positions must be greater than 0.");

            RuleFor(x => x.Request.MinSalary)
                .LessThan(x => x.Request.MaxSalary)
                .When(x => x.Request.MinSalary.HasValue && x.Request.MaxSalary.HasValue)
                .WithMessage("MinSalary must be less than MaxSalary.");

            RuleFor(x => x.Request.ClosingDate)
                .GreaterThan(x => x.Request.PostingDate)
                .When(x => x.Request.PostingDate.HasValue && x.Request.ClosingDate.HasValue)
                .WithMessage("Closing date must be after posting date.");

            RuleFor(x => x.Request.Location)
                .MaximumLength(200).WithMessage("Location must not exceed 200 characters.");

            RuleFor(x => x.Request.RequiredDistrict)
                .MaximumLength(100).WithMessage("RequiredDistrict must not exceed 100 characters.");

            RuleFor(x => x.Request.MinAge)
                .GreaterThan(0).When(x => x.Request.MinAge.HasValue)
                .WithMessage("MinAge must be greater than 0.");

            RuleFor(x => x.Request.MaxAge)
                .GreaterThan(0).When(x => x.Request.MaxAge.HasValue)
                .WithMessage("MaxAge must be greater than 0.");

            RuleFor(x => x.Request)
                .Must(r => !r.MinAge.HasValue || !r.MaxAge.HasValue || r.MinAge.Value <= r.MaxAge.Value)
                .WithMessage("MinAge must be less than or equal to MaxAge.")
                .OverridePropertyName("Request.MinAge");

            RuleFor(x => x.Request.MinExperienceYears)
                .GreaterThanOrEqualTo(0).When(x => x.Request.MinExperienceYears.HasValue)
                .WithMessage("MinExperienceYears must be greater than or equal to 0.");

            RuleFor(x => x.Request.ApplicationFeeAmount)
                .GreaterThanOrEqualTo(0).When(x => x.Request.ApplicationFeeAmount.HasValue)
                .WithMessage("ApplicationFeeAmount must be greater than or equal to 0.");

            RuleFor(x => x.Request.ApplicationFeeCurrency)
                .NotEmpty().When(x => x.Request.ApplicationFeeAmount.HasValue)
                .WithMessage("ApplicationFeeCurrency is required when ApplicationFeeAmount is set.");
        }
    }
}
