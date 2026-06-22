using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewVenues.Queries.InterviewVenueGetById
{
    public class InterviewVenueGetByIdHandler : IRequestHandler<InterviewVenueGetByIdQuery, InterviewVenueResponse>
    {
        private readonly IInterviewVenueService _service;

        public InterviewVenueGetByIdHandler(IInterviewVenueService service)
        {
            _service = service;
        }

        public async Task<InterviewVenueResponse> Handle(InterviewVenueGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.InterviewVenueId);
        }
    }
}
