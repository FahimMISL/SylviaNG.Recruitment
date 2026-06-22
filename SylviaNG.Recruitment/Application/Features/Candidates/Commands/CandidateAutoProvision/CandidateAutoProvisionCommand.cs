using MediatR;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateAutoProvision
{
    public class CandidateAutoProvisionCommand : IRequest<long>
    {
        public CandidateAutoProvisionRequest Request { get; set; }

        public CandidateAutoProvisionCommand(CandidateAutoProvisionRequest request)
        {
            Request = request;
        }
    }
}
