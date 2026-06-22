using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Queries.InterviewScorecardGetById
{
    public class InterviewScorecardGetByIdHandler : IRequestHandler<InterviewScorecardGetByIdQuery, InterviewScorecardResponse>
    {
        private readonly IInterviewScorecardService _service;

        public InterviewScorecardGetByIdHandler(IInterviewScorecardService service)
        {
            _service = service;
        }

        public async Task<InterviewScorecardResponse> Handle(InterviewScorecardGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.InterviewScorecardId);
        }
    }
}
