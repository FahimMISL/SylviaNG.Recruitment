using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Queries.InterviewSessionGetAll
{
    public class InterviewSessionGetAllHandler : IRequestHandler<InterviewSessionGetAllQuery, List<InterviewSessionResponse>>
    {
        private readonly IInterviewSessionService _service;

        public InterviewSessionGetAllHandler(IInterviewSessionService service)
        {
            _service = service;
        }

        public async Task<List<InterviewSessionResponse>> Handle(InterviewSessionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
