using SylviaNG.Recruitment.Application.Features.Genders.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class GenderMapper
    {
        public static Gender ToEntity(this GenderCreateRequest request)
        {
            return new Gender
            {
                Name = request.Name,
            };
        }

        public static GenderResponse ToResponse(this Gender entity)
        {
            return new GenderResponse
            {
                GenderId = entity.GenderId,
                Name = entity.Name,
            };
        }
    }
}
