using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletBulkGenerate
{
    // Empty-OfferLetterIds guard lives in JoiningBookletService.BulkGenerateAsync (matches the
    // CvBankCvBulkDownloadHandler precedent - no separate rule for that one check).
    public class JoiningBookletBulkGenerateValidator : AbstractValidator<JoiningBookletBulkGenerateCommand>
    {
        public JoiningBookletBulkGenerateValidator()
        {
            RuleFor(x => x.Request.DocumentTemplateId)
                .GreaterThan(0).WithMessage("DocumentTemplateId is required.");

            RuleFor(x => x.Request.BatchLabel)
                .NotEmpty().WithMessage("BatchLabel is required.");

            RuleFor(x => x.Request.JoiningDate)
                .NotEqual(default(DateTime)).WithMessage("JoiningDate is required.");
        }
    }
}
