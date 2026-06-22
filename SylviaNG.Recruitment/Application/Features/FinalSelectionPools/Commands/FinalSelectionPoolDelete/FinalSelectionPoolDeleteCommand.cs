using MediatR;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolDelete
{
    public class FinalSelectionPoolDeleteCommand : IRequest<Unit>
    {
        public long FinalSelectionPoolId { get; set; }

        public FinalSelectionPoolDeleteCommand(long finalSelectionPoolId)
        {
            FinalSelectionPoolId = finalSelectionPoolId;
        }
    }
}
