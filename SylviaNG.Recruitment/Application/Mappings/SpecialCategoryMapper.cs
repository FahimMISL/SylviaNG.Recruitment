using SylviaNG.Recruitment.Application.Features.SpecialCategories.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class SpecialCategoryMapper
    {
        public static SpecialCategory ToEntity(this SpecialCategoryCreateRequest request)
        {
            return new SpecialCategory
            {
                Name = request.Name,
            };
        }

        public static SpecialCategoryResponse ToResponse(this SpecialCategory entity)
        {
            return new SpecialCategoryResponse
            {
                SpecialCategoryId = entity.SpecialCategoryId,
                Name = entity.Name,
            };
        }
    }
}
