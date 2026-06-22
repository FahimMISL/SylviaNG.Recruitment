using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardCreate
{
    public class InterviewScorecardCreateCommand : IRequest<long>
    {
        public InterviewScorecardCreateRequest Request { get; set; }

        public InterviewScorecardCreateCommand(InterviewScorecardCreateRequest request)
        {
            Request = request;
        }
    }
}
