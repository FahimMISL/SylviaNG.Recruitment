using SylviaNG.Recruitment.Application.Features.Religions.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ReligionMapper
    {
        public static Religion ToEntity(this ReligionCreateRequest request)
        {
            return new Religion
            {
                Name = request.Name,
            };
        }

        public static ReligionResponse ToResponse(this Religion entity)
        {
            return new ReligionResponse
            {
                ReligionId = entity.ReligionId,
                Name = entity.Name,
            };
        }
    }
}
