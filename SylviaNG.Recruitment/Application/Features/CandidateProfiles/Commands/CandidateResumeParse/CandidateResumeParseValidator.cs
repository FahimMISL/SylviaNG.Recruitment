using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateResumeParse
{
    public class CandidateResumeParseValidator : AbstractValidator<CandidateResumeParseCommand>
    {
        private static readonly string[] AllowedExtensions = { ".pdf", ".docx" };
        private const int MaxFileSizeMB = 10;

        public CandidateResumeParseValidator()
        {
            var maxFileSizeBytes = MaxFileSizeMB * 1024L * 1024L;

            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(f => f != null && f.Length > 0).WithMessage("File must not be empty.");

            RuleFor(x => x.File)
                .Must(f => f != null && AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLowerInvariant()))
                .WithMessage($"File extension must be one of: {string.Join(", ", AllowedExtensions)}.")
                .When(x => x.File != null);

            RuleFor(x => x.File)
                .Must(f => f != null && f.Length <= maxFileSizeBytes)
                .WithMessage($"File size must not exceed {MaxFileSizeMB} MB.")
                .When(x => x.File != null);
        }
    }
}
