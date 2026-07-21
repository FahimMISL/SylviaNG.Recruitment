using FluentValidation;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit
{
    /// <summary>
    /// Validates the MediatR command (not the raw JobApplicationSubmitRequest DTO) - this app's
    /// FluentValidation auto-validation binds IValidator&lt;T&gt; to any type T, including raw
    /// controller-bound DTOs, which would bypass the {hasError,decentMessage,...} response wrapping
    /// if this validator targeted JobApplicationSubmitRequest directly. See JobPostingCreateValidator
    /// for the established precedent.
    /// </summary>
    public class JobApplicationSubmitValidator : AbstractValidator<JobApplicationSubmitCommand>
    {
        public JobApplicationSubmitValidator(IOptions<ApplicationCvStorageSettings> cvStorageOptions)
        {
            var settings = cvStorageOptions.Value;
            var maxFileSizeBytes = settings.MaxFileSizeMB * 1024L * 1024L;
            var allowedExtensions = settings.AllowedExtensions;

            RuleFor(x => x.Request.JobPostingId)
                .GreaterThan(0).WithMessage("JobPostingId is required.");

            RuleFor(x => x.Request.CandidateName)
                .NotEmpty().WithMessage("CandidateName is required.")
                .MaximumLength(200).WithMessage("CandidateName must not exceed 200 characters.");

            RuleFor(x => x.Request.CandidateEmail)
                .NotEmpty().WithMessage("CandidateEmail is required.")
                .EmailAddress().WithMessage("CandidateEmail must be a valid email address.")
                .MaximumLength(200).WithMessage("CandidateEmail must not exceed 200 characters.");

            RuleFor(x => x.Request.CandidatePhone)
                .MaximumLength(50).WithMessage("CandidatePhone must not exceed 50 characters.")
                .Matches(@"^[0-9+\-\s()]+$").WithMessage("CandidatePhone must be a valid phone number.")
                .When(x => !string.IsNullOrEmpty(x.Request.CandidatePhone));

            RuleFor(x => x.Request.CandidateNationalId)
                .MaximumLength(50).WithMessage("CandidateNationalId must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.Request.CandidateNationalId));

            RuleFor(x => x.Request.CoverLetter)
                .MaximumLength(5000).WithMessage("CoverLetter must not exceed 5000 characters.");

            RuleFor(x => x.Request.Resume)
                .NotNull().WithMessage("Resume is required.")
                .Must(f => f != null && f.Length > 0).WithMessage("Resume must not be empty.");

            RuleFor(x => x.Request.Resume)
                .Must(f => f != null && allowedExtensions.Contains(Path.GetExtension(f.FileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Resume extension must be one of: {string.Join(", ", allowedExtensions)}.")
                .When(x => x.Request.Resume != null);

            RuleFor(x => x.Request.Resume)
                .Must(f => f != null && f.Length <= maxFileSizeBytes)
                .WithMessage($"Resume size must not exceed {settings.MaxFileSizeMB} MB.")
                .When(x => x.Request.Resume != null);

            // US-005 AC3: internal candidates must attach a PDF specifically, not just any
            // allowed extension.
            RuleFor(x => x.Request.Resume)
                .Must(f => f != null && string.Equals(Path.GetExtension(f.FileName), ".pdf", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Internal candidates must attach their resume as a PDF file.")
                .When(x => x.Source == ApplicationSourceEnum.Internal && x.Request.Resume != null);
        }
    }
}
