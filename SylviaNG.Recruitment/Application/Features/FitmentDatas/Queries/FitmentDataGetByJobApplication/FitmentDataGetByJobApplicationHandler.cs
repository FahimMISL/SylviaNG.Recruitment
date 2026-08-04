using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetByJobApplication
{
    public class FitmentDataGetByJobApplicationHandler : IRequestHandler<FitmentDataGetByJobApplicationQuery, FitmentDataResponse?>
    {
        private readonly IFitmentDataService _fitmentDataService;

        public FitmentDataGetByJobApplicationHandler(IFitmentDataService fitmentDataService)
        {
            _fitmentDataService = fitmentDataService;
        }

        public async Task<FitmentDataResponse?> Handle(FitmentDataGetByJobApplicationQuery query, CancellationToken cancellationToken)
        {
            return await _fitmentDataService.GetByJobApplicationIdAsync(query.JobApplicationId);
        }
    }
}
