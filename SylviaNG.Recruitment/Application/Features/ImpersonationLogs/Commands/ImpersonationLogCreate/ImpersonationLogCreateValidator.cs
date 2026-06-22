using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogCreate
{
    public class ImpersonationLogCreateValidator : AbstractValidator<ImpersonationLogCreateCommand>
    {
        public ImpersonationLogCreateValidator()
        {
            RuleFor(x => x.Request.AdminUserId)
                .GreaterThan(0).WithMessage("AdminUserId is required.");

            RuleFor(x => x.Request.CandidateId)
                .GreaterThan(0).WithMessage("CandidateId is required.");

            RuleFor(x => x.Request.Reason)
                .NotEmpty().WithMessage("Reason is required.");
        }
    }
}
