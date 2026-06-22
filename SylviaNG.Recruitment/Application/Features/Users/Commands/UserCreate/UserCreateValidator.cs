using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Users.Commands.UserCreate
{
    public class UserCreateValidator : AbstractValidator<UserCreateCommand>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.Request.KeycloakUserId)
                .NotEmpty().WithMessage("KeycloakUserId is required.")
                .MaximumLength(200).WithMessage("KeycloakUserId must not exceed 200 characters.");

            RuleFor(x => x.Request.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(200).WithMessage("FullName must not exceed 200 characters.");

            RuleFor(x => x.Request.Email)
                .MaximumLength(200).WithMessage("Email must not exceed 200 characters.")
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Request.Email))
                .WithMessage("Email must be a valid email address.");
        }
    }
}
