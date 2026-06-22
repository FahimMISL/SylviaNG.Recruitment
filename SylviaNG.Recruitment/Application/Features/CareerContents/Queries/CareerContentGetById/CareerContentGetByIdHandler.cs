using MediatR;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Queries.CareerContentGetById
{
    public class CareerContentGetByIdHandler : IRequestHandler<CareerContentGetByIdQuery, CareerContentResponse>
    {
        private readonly ICareerContentService _service;

        public CareerContentGetByIdHandler(ICareerContentService service)
        {
            _service = service;
        }

        public async Task<CareerContentResponse> Handle(CareerContentGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.CareerContentId);
        }
    }
}
