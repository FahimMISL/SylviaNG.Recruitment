using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailUpdate
{
    public class AccountEmailUpdateValidator : AbstractValidator<AccountEmailUpdateCommand>
    {
        public AccountEmailUpdateValidator()
        {
            RuleFor(x => x.Request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");
        }
    }
}
