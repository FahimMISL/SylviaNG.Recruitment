using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillDelete
{
    public class CandidateSkillDeleteCommand : IRequest<Unit>
    {
        public long CandidateSkillId { get; set; }

        public CandidateSkillDeleteCommand(long candidateSkillId)
        {
            CandidateSkillId = candidateSkillId;
        }
    }
}
