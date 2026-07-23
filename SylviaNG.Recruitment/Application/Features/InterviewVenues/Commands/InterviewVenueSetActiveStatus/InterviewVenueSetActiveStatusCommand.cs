using MediatR;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueSetActiveStatus
{
    public class InterviewVenueSetActiveStatusCommand : IRequest
    {
        public long InterviewVenueId { get; set; }
        public bool IsActive { get; set; }

        public InterviewVenueSetActiveStatusCommand(long interviewVenueId, bool isActive)
        {
            InterviewVenueId = interviewVenueId;
            IsActive = isActive;
        }
    }
}
