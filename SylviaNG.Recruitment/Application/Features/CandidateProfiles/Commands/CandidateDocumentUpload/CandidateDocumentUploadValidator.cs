using FluentValidation;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateDocumentUpload
{
    public class CandidateDocumentUploadValidator : AbstractValidator<CandidateDocumentUploadCommand>
    {
        public CandidateDocumentUploadValidator(IOptions<CandidateDocumentSettings> options)
        {
            var settings = options.Value;
            var maxFileSizeBytes = settings.MaxFileSizeMB * 1024L * 1024L;

            RuleFor(x => x.Request.File)
                .NotNull().WithMessage("File is required.")
                .Must(f => f != null && f.Length > 0).WithMessage("File must not be empty.");

            RuleFor(x => x.Request.File)
                .Must(f => f != null && settings.AllowedExtensions.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"File extension must be one of: {string.Join(", ", settings.AllowedExtensions)}.")
                .When(x => x.Request.File != null);

            RuleFor(x => x.Request.File)
                .Must(f => f != null && f.Length <= maxFileSizeBytes)
                .WithMessage($"File size must not exceed {settings.MaxFileSizeMB} MB.")
                .When(x => x.Request.File != null);
        }
    }
}
