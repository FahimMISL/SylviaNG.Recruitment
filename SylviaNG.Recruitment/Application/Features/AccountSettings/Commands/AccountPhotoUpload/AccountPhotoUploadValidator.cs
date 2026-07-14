using FluentValidation;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPhotoUpload
{
    public class AccountPhotoUploadValidator : AbstractValidator<AccountPhotoUploadCommand>
    {
        public AccountPhotoUploadValidator(IOptions<CandidatePhotoSignatureSettings> options)
        {
            var settings = options.Value;
            var maxFileSizeBytes = settings.MaxFileSizeMB * 1024L * 1024L;

            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(f => f != null && f.Length > 0).WithMessage("File must not be empty.");

            RuleFor(x => x.File)
                .Must(f => f != null && settings.AllowedExtensions.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"File extension must be one of: {string.Join(", ", settings.AllowedExtensions)}.")
                .When(x => x.File != null);

            RuleFor(x => x.File)
                .Must(f => f != null && f.Length <= maxFileSizeBytes)
                .WithMessage($"File size must not exceed {settings.MaxFileSizeMB} MB.")
                .When(x => x.File != null);
        }
    }
}
