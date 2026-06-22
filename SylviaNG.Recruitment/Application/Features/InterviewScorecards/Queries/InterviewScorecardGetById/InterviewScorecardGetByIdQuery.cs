using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Queries.InterviewScorecardGetById
{
    public class InterviewScorecardGetByIdQuery : IRequest<InterviewScorecardResponse>
    {
        public long InterviewScorecardId { get; set; }

        public InterviewScorecardGetByIdQuery(long interviewScorecardId)
        {
            InterviewScorecardId = interviewScorecardId;
        }
    }
}
