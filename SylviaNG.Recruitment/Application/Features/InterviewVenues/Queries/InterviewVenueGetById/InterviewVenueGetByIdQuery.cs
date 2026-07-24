using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetById
{
    public class InterviewVenueGetByIdQuery : IRequest<InterviewVenueResponse>
    {
        public long InterviewVenueId { get; set; }

        public InterviewVenueGetByIdQuery(long interviewVenueId)
        {
            InterviewVenueId = interviewVenueId;
        }
    }
}
