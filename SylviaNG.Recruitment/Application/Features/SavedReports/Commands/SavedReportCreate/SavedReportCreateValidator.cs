using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportCreate
{
    public class SavedReportCreateValidator : AbstractValidator<SavedReportCreateCommand>
    {
        public SavedReportCreateValidator()
        {
            RuleFor(x => x.Request.CreatedByUserId)
                .GreaterThan(0).WithMessage("CreatedByUserId is required.");

            RuleFor(x => x.Request.ReportName)
                .NotEmpty().WithMessage("ReportName is required.")
                .MaximumLength(500).WithMessage("ReportName must not exceed 500 characters.");
        }
    }
}
