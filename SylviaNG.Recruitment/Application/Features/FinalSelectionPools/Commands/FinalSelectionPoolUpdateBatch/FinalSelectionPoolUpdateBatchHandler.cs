using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolUpdateBatch
{
    public class FinalSelectionPoolUpdateBatchHandler : IRequestHandler<FinalSelectionPoolUpdateBatchCommand, FinalSelectionPoolResponse>
    {
        private readonly IFinalSelectionPoolService _finalSelectionPoolService;

        public FinalSelectionPoolUpdateBatchHandler(IFinalSelectionPoolService finalSelectionPoolService)
        {
            _finalSelectionPoolService = finalSelectionPoolService;
        }

        public async Task<FinalSelectionPoolResponse> Handle(FinalSelectionPoolUpdateBatchCommand command, CancellationToken cancellationToken)
        {
            return await _finalSelectionPoolService.UpdateBatchAsync(command.FinalSelectionPoolId, command.Request);
        }
    }
}
