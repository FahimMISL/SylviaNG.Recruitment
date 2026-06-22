using MediatR;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueDelete
{
    public class InterviewVenueDeleteCommand : IRequest<Unit>
    {
        public long InterviewVenueId { get; set; }

        public InterviewVenueDeleteCommand(long interviewVenueId)
        {
            InterviewVenueId = interviewVenueId;
        }
    }
}
