using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Queries.InterviewGetById
{
    public class InterviewGetByIdHandler : IRequestHandler<InterviewGetByIdQuery, InterviewResponse>
    {
        private readonly IInterviewService _interviewService;

        public InterviewGetByIdHandler(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task<InterviewResponse> Handle(InterviewGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _interviewService.GetByIdAsync(query.InterviewId);
        }
    }
}
