using MediatR;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Queries.InterviewGetById
{
    public class InterviewGetByIdQuery : IRequest<InterviewResponse>
    {
        public long InterviewId { get; set; }

        public InterviewGetByIdQuery(long interviewId)
        {
            InterviewId = interviewId;
        }
    }
}
