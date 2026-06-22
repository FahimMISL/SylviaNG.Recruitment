using MediatR;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Queries.CareerContentGetAllPaged
{
    public class CareerContentGetAllPagedHandler : IRequestHandler<CareerContentGetAllPagedQuery, PagedResult<CareerContentResponse>>
    {
        private readonly ICareerContentService _careerContentService;

        public CareerContentGetAllPagedHandler(ICareerContentService careerContentService)
        {
            _careerContentService = careerContentService;
        }

        public async Task<PagedResult<CareerContentResponse>> Handle(CareerContentGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _careerContentService.GetPaginatedAsync(query.Request);
        }
    }
}
