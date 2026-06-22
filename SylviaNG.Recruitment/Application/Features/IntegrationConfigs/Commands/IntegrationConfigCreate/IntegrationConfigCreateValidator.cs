using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigCreate
{
    public class IntegrationConfigCreateValidator : AbstractValidator<IntegrationConfigCreateCommand>
    {
        public IntegrationConfigCreateValidator()
        {
            RuleFor(x => x.Request.ConfigName)
                .NotEmpty().WithMessage("ConfigName is required.")
                .MaximumLength(500).WithMessage("ConfigName must not exceed 500 characters.");
        }
    }
}
