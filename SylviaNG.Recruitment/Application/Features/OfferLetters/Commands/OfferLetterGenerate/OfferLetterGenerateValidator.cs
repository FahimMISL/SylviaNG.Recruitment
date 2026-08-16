using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterGenerate
{
    public class OfferLetterGenerateValidator : AbstractValidator<OfferLetterGenerateCommand>
    {
        public OfferLetterGenerateValidator()
        {
            RuleFor(x => x.Request.JobApplicationId)
                .GreaterThan(0).WithMessage("JobApplicationId is required.");

            RuleFor(x => x.Request.DocumentTemplateId)
                .GreaterThan(0).WithMessage("DocumentTemplateId is required.");

            RuleFor(x => x.Request.Designation)
                .NotEmpty().WithMessage("Designation is required.")
                .MaximumLength(200).WithMessage("Designation must not exceed 200 characters.");

            RuleFor(x => x.Request.OfferedSalary)
                .GreaterThan(0).WithMessage("OfferedSalary must be greater than 0.");

            RuleFor(x => x.Request.JoiningDate)
                .NotEmpty().WithMessage("JoiningDate is required.");

            RuleFor(x => x.Request.ReportingManager)
                .MaximumLength(200).WithMessage("ReportingManager must not exceed 200 characters.");
        }
    }
}
