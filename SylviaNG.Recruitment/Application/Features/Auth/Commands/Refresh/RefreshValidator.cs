using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.Refresh
{
    public class RefreshValidator : AbstractValidator<RefreshCommand>
    {
        public RefreshValidator()
        {
            RuleFor(x => x.Request.RefreshToken)
                .NotEmpty().WithMessage("RefreshToken is required.");
        }
    }
}
