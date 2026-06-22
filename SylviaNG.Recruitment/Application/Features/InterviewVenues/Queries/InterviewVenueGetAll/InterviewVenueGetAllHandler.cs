using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetAll
{
    public class InterviewVenueGetAllHandler : IRequestHandler<InterviewVenueGetAllQuery, List<InterviewVenueResponse>>
    {
        private readonly IInterviewVenueService _service;

        public InterviewVenueGetAllHandler(IInterviewVenueService service)
        {
            _service = service;
        }

        public async Task<List<InterviewVenueResponse>> Handle(InterviewVenueGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
