using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueCreate
{
    public class InterviewVenueCreateValidator : AbstractValidator<InterviewVenueCreateCommand>
    {
        public InterviewVenueCreateValidator()
        {
            RuleFor(x => x.Request.VenueName)
                .NotEmpty().WithMessage("VenueName is required.")
                .MaximumLength(500).WithMessage("VenueName must not exceed 500 characters.");
        }
    }
}
