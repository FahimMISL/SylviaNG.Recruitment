using MediatR;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateCreate
{
    public class CandidateCreateCommand : IRequest<long>
    {
        public CandidateCreateRequest Request { get; set; }

        public CandidateCreateCommand(CandidateCreateRequest request)
        {
            Request = request;
        }
    }
}
