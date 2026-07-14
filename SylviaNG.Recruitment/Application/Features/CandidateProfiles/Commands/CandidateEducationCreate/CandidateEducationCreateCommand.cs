using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateEducationCreate
{
    public class CandidateEducationCreateCommand : IRequest<long>
    {
        public CandidateEducationCreateRequest Request { get; set; }

        public CandidateEducationCreateCommand(CandidateEducationCreateRequest request)
        {
            Request = request;
        }
    }
}
