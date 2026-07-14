using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileHrNotesUpdate
{
    public class CandidateProfileHrNotesUpdateValidator : AbstractValidator<CandidateProfileHrNotesUpdateCommand>
    {
        public CandidateProfileHrNotesUpdateValidator()
        {
            RuleFor(x => x.HrNotes)
                .MaximumLength(2000).WithMessage("HrNotes must not exceed 2000 characters.");
        }
    }
}
