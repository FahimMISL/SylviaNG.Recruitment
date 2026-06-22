using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillCreate
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
