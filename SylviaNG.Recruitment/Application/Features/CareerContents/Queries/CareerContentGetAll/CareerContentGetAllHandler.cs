using MediatR;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Queries.CareerContentGetAll
{
    public class CareerContentGetAllHandler : IRequestHandler<CareerContentGetAllQuery, List<CareerContentResponse>>
    {
        private readonly ICareerContentService _service;

        public CareerContentGetAllHandler(ICareerContentService service)
        {
            _service = service;
        }

        public async Task<List<CareerContentResponse>> Handle(CareerContentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
