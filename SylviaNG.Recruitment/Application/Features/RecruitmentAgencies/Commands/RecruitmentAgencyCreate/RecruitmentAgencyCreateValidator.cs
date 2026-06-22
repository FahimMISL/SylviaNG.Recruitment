using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyCreate
{
    public class RecruitmentAgencyCreateValidator : AbstractValidator<RecruitmentAgencyCreateCommand>
    {
        public RecruitmentAgencyCreateValidator()
        {
            RuleFor(x => x.Request.AgencyName)
                .NotEmpty().WithMessage("AgencyName is required.")
                .MaximumLength(500).WithMessage("AgencyName must not exceed 500 characters.");
        }
    }
}
