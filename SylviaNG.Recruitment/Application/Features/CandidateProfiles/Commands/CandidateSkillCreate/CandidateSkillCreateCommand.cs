using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateSkillCreate
{
    public class CandidateSkillCreateCommand : IRequest<long>
    {
        public CandidateSkillCreateRequest Request { get; set; }

        public CandidateSkillCreateCommand(CandidateSkillCreateRequest request)
        {
            Request = request;
        }
    }
}
