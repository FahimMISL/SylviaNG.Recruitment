using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolUpdate
{
    public class FinalSelectionPoolUpdateCommand : IRequest<Unit>
    {
        public long FinalSelectionPoolId { get; set; }
        public FinalSelectionPoolUpdateRequest Request { get; set; }

        public FinalSelectionPoolUpdateCommand(long finalSelectionPoolId, FinalSelectionPoolUpdateRequest request)
        {
            FinalSelectionPoolId = finalSelectionPoolId;
            Request = request;
        }
    }
}
