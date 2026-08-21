using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueUpdate
{
    public class InterviewVenueUpdateCommand : IRequest
    {
        public long InterviewVenueId { get; set; }
        public InterviewVenueUpdateRequest Request { get; set; }

        public InterviewVenueUpdateCommand(long interviewVenueId, InterviewVenueUpdateRequest request)
        {
            InterviewVenueId = interviewVenueId;
            Request = request;
        }
    }
}
