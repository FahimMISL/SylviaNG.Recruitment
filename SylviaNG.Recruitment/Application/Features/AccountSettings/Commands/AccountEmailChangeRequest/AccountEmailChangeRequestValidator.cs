using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailChangeRequest
{
    public class AccountEmailChangeRequestValidator : AbstractValidator<AccountEmailChangeRequestCommand>
    {
        public AccountEmailChangeRequestValidator()
        {
            RuleFor(x => x.Request.NewEmail)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");
        }
    }
}
