using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigCreate
{
    public class ProfileFieldConfigCreateValidator : AbstractValidator<ProfileFieldConfigCreateCommand>
    {
        public ProfileFieldConfigCreateValidator()
        {
            RuleFor(x => x.Request.FieldName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Request.Label).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Request.FieldType).NotEmpty().MaximumLength(50);
        }
    }
}
