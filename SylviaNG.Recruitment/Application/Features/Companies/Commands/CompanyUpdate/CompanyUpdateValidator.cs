using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Companies.Commands.CompanyUpdate
{
    public class CompanyUpdateValidator : AbstractValidator<CompanyUpdateCommand>
    {
        public CompanyUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(200).WithMessage("Company name must not exceed 200 characters.");

            RuleFor(x => x.Request.Email)
                .EmailAddress().WithMessage("A valid email is required.")
                .When(x => !string.IsNullOrEmpty(x.Request.Email));

            RuleFor(x => x.Request.Website)
                .MaximumLength(200).WithMessage("Website must not exceed 200 characters.");
        }
    }
}
