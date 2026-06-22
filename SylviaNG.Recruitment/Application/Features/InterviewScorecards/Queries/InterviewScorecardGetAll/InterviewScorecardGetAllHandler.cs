using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Queries.InterviewScorecardGetAll
{
    public class InterviewScorecardGetAllHandler : IRequestHandler<InterviewScorecardGetAllQuery, List<InterviewScorecardResponse>>
    {
        private readonly IInterviewScorecardService _service;

        public InterviewScorecardGetAllHandler(IInterviewScorecardService service)
        {
            _service = service;
        }

        public async Task<List<InterviewScorecardResponse>> Handle(InterviewScorecardGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
