using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolMarkHasJoined
{
    public class FinalSelectionPoolMarkHasJoinedHandler : IRequestHandler<FinalSelectionPoolMarkHasJoinedCommand, FinalSelectionPoolResponse>
    {
        private readonly IFinalSelectionPoolService _finalSelectionPoolService;

        public FinalSelectionPoolMarkHasJoinedHandler(IFinalSelectionPoolService finalSelectionPoolService)
        {
            _finalSelectionPoolService = finalSelectionPoolService;
        }

        public async Task<FinalSelectionPoolResponse> Handle(FinalSelectionPoolMarkHasJoinedCommand command, CancellationToken cancellationToken)
        {
            return await _finalSelectionPoolService.MarkHasJoinedAsync(command.FinalSelectionPoolId);
        }
    }
}
