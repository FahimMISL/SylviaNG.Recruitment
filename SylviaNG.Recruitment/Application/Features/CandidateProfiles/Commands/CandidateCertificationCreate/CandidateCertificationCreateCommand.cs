using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateCertificationCreate
{
    public class CandidateCertificationCreateCommand : IRequest<long>
    {
        public CandidateCertificationCreateRequest Request { get; set; }

        public CandidateCertificationCreateCommand(CandidateCertificationCreateRequest request)
        {
            Request = request;
        }
    }
}
