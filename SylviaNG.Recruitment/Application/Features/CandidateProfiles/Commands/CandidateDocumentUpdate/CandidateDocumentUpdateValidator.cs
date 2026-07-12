using FluentValidation;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentUpdate
{
    public class CandidateDocumentUpdateValidator : AbstractValidator<CandidateDocumentUpdateCommand>
    {
        public CandidateDocumentUpdateValidator(IOptions<CandidateDocumentSettings> options)
        {
            var settings = options.Value;
            var maxFileSizeBytes = settings.MaxFileSizeMB * 1024L * 1024L;

            RuleFor(x => x.CandidateDocumentId)
                .GreaterThan(0).WithMessage("CandidateDocumentId is required.");

            RuleFor(x => x.Request.File)
                .Must(f => f == null || settings.AllowedExtensions.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"File extension must be one of: {string.Join(", ", settings.AllowedExtensions)}.");

            RuleFor(x => x.Request.File)
                .Must(f => f == null || f.Length <= maxFileSizeBytes)
                .WithMessage($"File size must not exceed {settings.MaxFileSizeMB} MB.");
        }
    }
}
