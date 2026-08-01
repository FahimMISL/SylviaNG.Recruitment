using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataUpsert
{
    public class FitmentDataUpsertHandler : IRequestHandler<FitmentDataUpsertCommand, FitmentDataResponse>
    {
        private readonly IFitmentDataService _fitmentDataService;

        public FitmentDataUpsertHandler(IFitmentDataService fitmentDataService)
        {
            _fitmentDataService = fitmentDataService;
        }

        public async Task<FitmentDataResponse> Handle(FitmentDataUpsertCommand command, CancellationToken cancellationToken)
        {
            return await _fitmentDataService.UpsertAsync(command.Request);
        }
    }
}
