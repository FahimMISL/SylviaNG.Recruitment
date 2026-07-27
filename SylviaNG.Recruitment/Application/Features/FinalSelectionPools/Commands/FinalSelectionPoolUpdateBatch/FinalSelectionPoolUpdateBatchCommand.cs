using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolUpdateBatch
{
    public class FinalSelectionPoolUpdateBatchCommand : IRequest<FinalSelectionPoolResponse>
    {
        public long FinalSelectionPoolId { get; set; }
        public FinalSelectionPoolUpdateBatchRequest Request { get; set; }

        public FinalSelectionPoolUpdateBatchCommand(long finalSelectionPoolId, FinalSelectionPoolUpdateBatchRequest request)
        {
            FinalSelectionPoolId = finalSelectionPoolId;
            Request = request;
        }
    }
}
