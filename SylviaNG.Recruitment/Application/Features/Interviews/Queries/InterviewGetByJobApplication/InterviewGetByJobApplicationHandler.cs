using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Queries.InterviewGetByJobApplication
{
    public class InterviewGetByJobApplicationHandler : IRequestHandler<InterviewGetByJobApplicationQuery, List<InterviewResponse>>
    {
        private readonly IInterviewService _interviewService;

        public InterviewGetByJobApplicationHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task<List<InterviewResponse>> Handle(InterviewGetByJobApplicationQuery query, CancellationToken cancellationToken)
        {
            return await _interviewService.GetByJobApplicationIdAsync(query.JobApplicationId);
        }
    }
}
