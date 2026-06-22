using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetById
{
    public class FitmentDataGetByIdHandler : IRequestHandler<FitmentDataGetByIdQuery, FitmentDataResponse>
    {
        private readonly IFitmentDataService _service;

        public FitmentDataGetByIdHandler(IFitmentDataService service)
        {
            _service = service;
        }

        public async Task<FitmentDataResponse> Handle(FitmentDataGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.FitmentDataId);
        }
    }
}
