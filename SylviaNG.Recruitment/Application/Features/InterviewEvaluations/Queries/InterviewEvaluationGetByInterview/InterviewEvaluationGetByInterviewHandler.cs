using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetByInterview
{
    public class InterviewEvaluationGetByInterviewHandler : IRequestHandler<InterviewEvaluationGetByInterviewQuery, List<InterviewEvaluationResponse>>
    {
        private readonly IInterviewEvaluationService _interviewEvaluationService;

        public InterviewEvaluationGetByInterviewHandler(IInterviewEvaluationService interviewEvaluationService)
        {
            _interviewEvaluationService = interviewEvaluationService;
        }

        public async Task<List<InterviewEvaluationResponse>> Handle(InterviewEvaluationGetByInterviewQuery query, CancellationToken cancellationToken)
        {
            return await _interviewEvaluationService.GetByInterviewIdAsync(query.InterviewId);
        }
    }
}
