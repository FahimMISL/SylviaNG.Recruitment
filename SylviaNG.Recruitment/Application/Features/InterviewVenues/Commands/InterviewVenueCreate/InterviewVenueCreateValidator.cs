using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueCreate
{
    public class InterviewVenueCreateValidator : AbstractValidator<InterviewVenueCreateCommand>
    {
        public InterviewVenueCreateValidator()
        {
            RuleFor(x => x.Request.VenueName)
                .NotEmpty().WithMessage("Venue name is required.")
                .MaximumLength(200).WithMessage("Venue name must not exceed 200 characters.");

            RuleFor(x => x.Request.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(300).WithMessage("Location must not exceed 300 characters.");
        }
    }
}
