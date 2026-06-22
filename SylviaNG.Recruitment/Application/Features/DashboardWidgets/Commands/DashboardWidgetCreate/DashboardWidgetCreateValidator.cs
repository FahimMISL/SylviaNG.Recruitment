using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.DashboardWidgets.Commands.DashboardWidgetCreate
{
    public class DashboardWidgetCreateValidator : AbstractValidator<DashboardWidgetCreateCommand>
    {
        public DashboardWidgetCreateValidator()
        {
            RuleFor(x => x.Request.UserId)
                .GreaterThan(0).WithMessage("UserId is required.");

            RuleFor(x => x.Request.WidgetType)
                .NotEmpty().WithMessage("WidgetType is required.")
                .MaximumLength(500).WithMessage("WidgetType must not exceed 500 characters.");
        }
    }
}
