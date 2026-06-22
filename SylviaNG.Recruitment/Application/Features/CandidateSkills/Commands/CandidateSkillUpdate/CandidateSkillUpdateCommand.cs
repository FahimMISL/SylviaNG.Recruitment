using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillUpdate
{
    public class CandidateSkillUpdateCommand : IRequest<Unit>
    {
        public long CandidateSkillId { get; set; }
        public CandidateSkillUpdateRequest Request { get; set; }

        public CandidateSkillUpdateCommand(long candidateSkillId, CandidateSkillUpdateRequest request)
        {
            CandidateSkillId = candidateSkillId;
            Request = request;
        }
    }
}
