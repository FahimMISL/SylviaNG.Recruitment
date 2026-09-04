using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolMarkHasJoined
{
    public class FinalSelectionPoolMarkHasJoinedCommand : IRequest<FinalSelectionPoolResponse>
    {
        public long FinalSelectionPoolId { get; set; }

        public FinalSelectionPoolMarkHasJoinedCommand(long finalSelectionPoolId)
        {
            FinalSelectionPoolId = finalSelectionPoolId;
        }
    }
}
