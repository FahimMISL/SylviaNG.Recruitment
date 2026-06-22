using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreate
{
    public class ExportRequestCreateValidator : AbstractValidator<ExportRequestCreateCommand>
    {
        public ExportRequestCreateValidator()
        {
            RuleFor(x => x.Request.ExportType).NotEmpty().WithMessage("ExportType is required.");
        }
    }
}
