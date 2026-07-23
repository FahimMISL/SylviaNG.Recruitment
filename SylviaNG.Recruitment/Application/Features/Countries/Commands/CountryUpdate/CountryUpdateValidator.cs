using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Countries.Commands.CountryUpdate
{
    public class CountryUpdateValidator : AbstractValidator<CountryUpdateCommand>
    {
        public CountryUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Request.Code)
                .NotEmpty().WithMessage("ISO code is required.")
                .MaximumLength(5).WithMessage("ISO code must not exceed 5 characters.");

            RuleFor(x => x.Request.DialCode)
                .NotEmpty().WithMessage("Dial code is required.")
                .MaximumLength(10).WithMessage("Dial code must not exceed 10 characters.")
                .Matches(@"^\+\d{1,4}$").WithMessage("Dial code must be in the form +<digits>, e.g. +880.");
        }
    }
}
