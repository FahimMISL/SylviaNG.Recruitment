using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanyCreate
{
    public class CompanyCreateValidator : AbstractValidator<CompanyCreateCommand>
    {
        public CompanyCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(200).WithMessage("Company name must not exceed 200 characters.");

            RuleFor(x => x.Request.Email)
                .EmailAddress().WithMessage("A valid email is required.")
                .When(x => !string.IsNullOrEmpty(x.Request.Email));

            RuleFor(x => x.Request.Website)
                .MaximumLength(200).WithMessage("Website must not exceed 200 characters.");

            RuleFor(x => x.Request.TradeLicenseNumber)
                .MaximumLength(100).WithMessage("Trade license number must not exceed 100 characters.");

            RuleFor(x => x.Request.BinNumber)
                .MaximumLength(100).WithMessage("BIN must not exceed 100 characters.");
        }
    }
}
