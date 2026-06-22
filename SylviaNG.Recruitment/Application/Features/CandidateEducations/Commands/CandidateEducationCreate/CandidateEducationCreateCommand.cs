using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Commands.CandidateEducationCreate
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
