using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardCreate
{
    public class AdmitCardCreateValidator : AbstractValidator<AdmitCardCreateCommand>
    {
        public AdmitCardCreateValidator()
        {
            RuleFor(x => x.Request.ExamCandidateId).GreaterThan(0).WithMessage("ExamCandidateId is required.");
        }
    }
}
