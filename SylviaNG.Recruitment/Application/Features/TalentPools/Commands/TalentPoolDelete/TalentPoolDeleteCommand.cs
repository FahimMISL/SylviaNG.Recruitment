using MediatR;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolDelete
{
    public class TalentPoolDeleteCommand : IRequest<Unit>
    {
        public long TalentPoolId { get; set; }

        public TalentPoolDeleteCommand(long talentPoolId)
        {
            TalentPoolId = talentPoolId;
        }
    }
}
