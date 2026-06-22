using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewSessions.Queries.InterviewSessionGetById
{
    public class InterviewSessionGetByIdHandler : IRequestHandler<InterviewSessionGetByIdQuery, InterviewSessionResponse>
    {
        private readonly IInterviewSessionService _service;

        public InterviewSessionGetByIdHandler(IInterviewSessionService service)
        {
            _service = service;
        }

        public async Task<InterviewSessionResponse> Handle(InterviewSessionGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.InterviewSessionId);
        }
    }
}
