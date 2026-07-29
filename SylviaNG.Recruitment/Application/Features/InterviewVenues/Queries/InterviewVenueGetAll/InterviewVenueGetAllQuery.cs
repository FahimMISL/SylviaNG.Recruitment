using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetAll
{
    public class InterviewVenueGetAllQuery : IRequest<List<InterviewVenueResponse>>
    {
    }
}
