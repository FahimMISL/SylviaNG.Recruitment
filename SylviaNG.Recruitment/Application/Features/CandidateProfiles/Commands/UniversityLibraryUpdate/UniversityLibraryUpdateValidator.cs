using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryUpdate
{
    public class UniversityLibraryUpdateValidator : AbstractValidator<UniversityLibraryUpdateCommand>
    {
        public UniversityLibraryUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("University name is required.")
                .MaximumLength(200).WithMessage("University name must not exceed 200 characters.");

            RuleFor(x => x.Request.Code)
                .NotEmpty().WithMessage("University code is required.")
                .MaximumLength(20).WithMessage("University code must not exceed 20 characters.");
        }
    }
}
