using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Commands.OfficeNoteGenerate
{
    public class OfficeNoteGenerateValidator : AbstractValidator<OfficeNoteGenerateCommand>
    {
        public OfficeNoteGenerateValidator()
        {
            RuleFor(x => x.Request.JobApplicationId).GreaterThan(0);
            RuleFor(x => x.Request.DocumentTemplateId).GreaterThan(0);
            RuleFor(x => x.Request.Remarks).MaximumLength(2000);
        }
    }
}
