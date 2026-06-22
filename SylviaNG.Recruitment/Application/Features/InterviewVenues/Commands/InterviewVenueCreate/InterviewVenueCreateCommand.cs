using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Commands.InterviewVenueCreate
{
    public class InterviewVenueCreateCommand : IRequest<long>
    {
        public InterviewVenueCreateRequest Request { get; set; }

        public InterviewVenueCreateCommand(InterviewVenueCreateRequest request)
        {
            Request = request;
        }
    }
}
