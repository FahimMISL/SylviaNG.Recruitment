using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetAll
{
    public class FitmentDataGetAllHandler : IRequestHandler<FitmentDataGetAllQuery, List<FitmentDataResponse>>
    {
        private readonly IFitmentDataService _service;

        public FitmentDataGetAllHandler(IFitmentDataService service)
        {
            _service = service;
        }

        public async Task<List<FitmentDataResponse>> Handle(FitmentDataGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
