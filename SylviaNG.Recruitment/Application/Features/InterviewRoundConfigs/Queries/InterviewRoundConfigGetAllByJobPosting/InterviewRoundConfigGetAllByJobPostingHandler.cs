using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Queries.InterviewRoundConfigGetAllByJobPosting
{
    public class InterviewRoundConfigGetAllByJobPostingHandler : IRequestHandler<InterviewRoundConfigGetAllByJobPostingQuery, List<InterviewRoundConfigResponse>>
    {
        private readonly IInterviewRoundConfigService _interviewRoundConfigService;

        public InterviewRoundConfigGetAllByJobPostingHandler(IInterviewRoundConfigService interviewRoundConfigService)
        {
            _interviewRoundConfigService = interviewRoundConfigService;
        }

        public async Task<List<InterviewRoundConfigResponse>> Handle(InterviewRoundConfigGetAllByJobPostingQuery query, CancellationToken cancellationToken)
        {
            return await _interviewRoundConfigService.GetAllByJobPostingIdAsync(query.JobPostingId);
        }
    }
}
