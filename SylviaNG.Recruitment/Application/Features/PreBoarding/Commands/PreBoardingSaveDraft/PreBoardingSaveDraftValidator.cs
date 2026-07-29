using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingSaveDraft
{
    public class PreBoardingSaveDraftValidator : AbstractValidator<PreBoardingSaveDraftCommand>
    {
        public PreBoardingSaveDraftValidator()
        {
            // AC: a draft can be saved with partial data, so only structural limits are enforced
            // here - completeness is validated later, at submit time (PreBoardingService.
            // EnsureReadyToSubmit).
            RuleFor(x => x.Request.EmergencyContactName).MaximumLength(200);
            RuleFor(x => x.Request.EmergencyContactRelationship).MaximumLength(100);
            RuleFor(x => x.Request.EmergencyContactPhone).MaximumLength(20);
            RuleFor(x => x.Request.InsuranceProvider).MaximumLength(200);
            RuleFor(x => x.Request.InsurancePolicyNumber).MaximumLength(100);
            RuleFor(x => x.Request.InsuranceNotes).MaximumLength(1000);
            RuleFor(x => x.Request.BankName).MaximumLength(200);
            RuleFor(x => x.Request.BankBranch).MaximumLength(200);
            RuleFor(x => x.Request.BankAccountName).MaximumLength(200);
            RuleFor(x => x.Request.BankAccountNumber).MaximumLength(50);
            RuleFor(x => x.Request.BankRoutingNumber).MaximumLength(50);

            RuleForEach(x => x.Request.Nominees).ChildRules(nominee =>
            {
                nominee.RuleFor(n => n.FullName).MaximumLength(200);
                nominee.RuleFor(n => n.Relationship).MaximumLength(100);
                nominee.RuleFor(n => n.SharePercentage).InclusiveBetween(0, 100)
                    .WithMessage("SharePercentage must be between 0 and 100.");
            });
        }
    }
}
