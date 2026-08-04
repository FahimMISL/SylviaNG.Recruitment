using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataUpsert
{
    public class FitmentDataUpsertValidator : AbstractValidator<FitmentDataUpsertCommand>
    {
        public FitmentDataUpsertValidator()
        {
            RuleFor(x => x.Request.JobApplicationId).GreaterThan(0);
            RuleFor(x => x.Request.Designation).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Request.Grade).MaximumLength(100);
            RuleFor(x => x.Request.Location).MaximumLength(200);
            RuleFor(x => x.Request.BasicSalary).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Request.TotalAllowances).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Request.TotalDeductions).GreaterThanOrEqualTo(0);
        }
    }
}
