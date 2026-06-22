using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Commands.CandidateCertificationCreate
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
