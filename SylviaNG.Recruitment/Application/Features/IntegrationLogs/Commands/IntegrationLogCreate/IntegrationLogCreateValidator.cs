using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogCreate
{
    public class IntegrationLogCreateValidator : AbstractValidator<IntegrationLogCreateCommand>
    {
        public IntegrationLogCreateValidator()
        {
            RuleFor(x => x.Request.OperationName)
                .NotEmpty().WithMessage("OperationName is required.")
                .MaximumLength(500).WithMessage("OperationName must not exceed 500 characters.");
        }
    }
}
