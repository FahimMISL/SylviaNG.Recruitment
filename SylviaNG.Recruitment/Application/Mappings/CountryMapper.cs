using SylviaNG.Recruitment.Application.Features.Countries.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CountryMapper
    {
        public static Country ToEntity(this CountryCreateRequest request)
        {
            return new Country
            {
                Name = request.Name,
                Code = request.Code,
                DialCode = request.DialCode,
            };
        }

        public static CountryResponse ToResponse(this Country entity)
        {
            return new CountryResponse
            {
                CountryId = entity.CountryId,
                Name = entity.Name,
                Code = entity.Code,
                DialCode = entity.DialCode,
            };
        }
    }
}
