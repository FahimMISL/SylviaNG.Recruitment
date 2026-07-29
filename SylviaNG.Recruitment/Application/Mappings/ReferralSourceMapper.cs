using SylviaNG.Recruitment.Application.Features.ReferralSources.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ReferralSourceMapper
    {
        public static ReferralSource ToEntity(this ReferralSourceCreateRequest request)
        {
            return new ReferralSource
            {
                Name = request.Name,
            };
        }

        public static ReferralSourceResponse ToResponse(this ReferralSource entity)
        {
            return new ReferralSourceResponse
            {
                ReferralSourceId = entity.ReferralSourceId,
                Name = entity.Name,
            };
        }
    }
}
