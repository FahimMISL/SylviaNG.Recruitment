using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetActiveLookup
{
    public class InterviewVenueGetActiveLookupQuery : IRequest<List<InterviewVenueLookupResponse>>
    {
    }
}
