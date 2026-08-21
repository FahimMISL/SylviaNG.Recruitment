using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Request.Username)
                .NotEmpty().WithMessage("Username is required.");
        }
    }
}
