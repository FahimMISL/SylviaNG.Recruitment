using MediatR;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SpecialCategories.Queries.SpecialCategoryGetAll
{
    public class SpecialCategoryGetAllHandler : IRequestHandler<SpecialCategoryGetAllQuery, List<SpecialCategoryResponse>>
    {
        private readonly ISpecialCategoryService _specialCategoryService;

        public SpecialCategoryGetAllHandler(ISpecialCategoryService specialCategoryService)
        {
            _specialCategoryService = specialCategoryService;
        }

        public async Task<List<SpecialCategoryResponse>> Handle(SpecialCategoryGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _specialCategoryService.GetAllAsync();
        }
    }
}
