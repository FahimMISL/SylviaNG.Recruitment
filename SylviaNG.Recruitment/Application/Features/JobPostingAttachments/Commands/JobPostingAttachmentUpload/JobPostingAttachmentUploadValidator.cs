using FluentValidation;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;

namespace SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Commands.JobPostingAttachmentUpload
{
    public class JobPostingAttachmentUploadValidator : AbstractValidator<JobPostingAttachmentUploadCommand>
    {
        public JobPostingAttachmentUploadValidator(IOptions<FileStorageSettings> fileStorageOptions)
        {
            var settings = fileStorageOptions.Value;
            var maxFileSizeBytes = settings.MaxFileSizeMB * 1024L * 1024L;
            var allowedExtensions = settings.AllowedExtensions;

            RuleFor(x => x.JobPostingId)
                .GreaterThan(0).WithMessage("JobPostingId is required.");

            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(f => f != null && f.Length > 0).WithMessage("File must not be empty.");

            RuleFor(x => x.File)
                .Must(f => f != null && allowedExtensions.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"File extension must be one of: {string.Join(", ", allowedExtensions)}.")
                .When(x => x.File != null);

            RuleFor(x => x.File)
                .Must(f => f != null && f.Length <= maxFileSizeBytes)
                .WithMessage($"File size must not exceed {settings.MaxFileSizeMB} MB.")
                .When(x => x.File != null);
        }
    }
}
