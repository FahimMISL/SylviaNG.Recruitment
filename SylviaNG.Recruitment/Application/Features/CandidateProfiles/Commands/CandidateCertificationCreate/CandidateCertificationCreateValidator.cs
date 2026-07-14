using FluentValidation;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationCreate
{
    public class CandidateCertificationCreateValidator : AbstractValidator<CandidateCertificationCreateCommand>
    {
        public CandidateCertificationCreateValidator(IOptions<FileStorageSettings> fileStorageOptions)
        {
            var settings = fileStorageOptions.Value;
            var maxFileSizeBytes = settings.MaxFileSizeMB * 1024L * 1024L;

            RuleFor(x => x.Request.CertificationName)
                .NotEmpty().WithMessage("CertificationName is required.")
                .MaximumLength(200).WithMessage("CertificationName must not exceed 200 characters.");

            RuleFor(x => x.Request.IssuingOrganization)
                .MaximumLength(200).WithMessage("IssuingOrganization must not exceed 200 characters.");

            RuleFor(x => x.Request.CredentialId)
                .MaximumLength(100).WithMessage("CredentialId must not exceed 100 characters.");

            RuleFor(x => x.Request.ExpiryDate)
                .GreaterThan(x => x.Request.IssueDate).WithMessage("ExpiryDate must be after IssueDate.")
                .When(x => x.Request.IssueDate.HasValue && x.Request.ExpiryDate.HasValue);

            RuleFor(x => x.Request.CertificateFile)
                .Must(f => f == null || settings.AllowedExtensions.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"CertificateFile extension must be one of: {string.Join(", ", settings.AllowedExtensions)}.");

            RuleFor(x => x.Request.CertificateFile)
                .Must(f => f == null || f.Length <= maxFileSizeBytes)
                .WithMessage($"CertificateFile size must not exceed {settings.MaxFileSizeMB} MB.");
        }
    }
}
