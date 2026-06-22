using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ShortlistFilterMapper
    {
        public static ShortlistFilter ToEntity(this ShortlistFilterCreateRequest request)
        {
            return new ShortlistFilter
            {
                RequisitionId = request.RequisitionId,
                FilterName = request.FilterName,
                IsAutoShortlistEnabled = request.IsAutoShortlistEnabled,
                RunOnSubmission = request.RunOnSubmission,
            };
        }

        public static void ApplyUpdate(this ShortlistFilter entity, ShortlistFilterUpdateRequest request)
        {
            if (request.RequisitionId.HasValue) entity.RequisitionId = request.RequisitionId.Value;
            if (request.FilterName is not null) entity.FilterName = request.FilterName;
            if (request.IsAutoShortlistEnabled.HasValue) entity.IsAutoShortlistEnabled = request.IsAutoShortlistEnabled.Value;
            if (request.RunOnSubmission.HasValue) entity.RunOnSubmission = request.RunOnSubmission.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ShortlistFilterResponse ToResponse(this ShortlistFilter entity)
        {
            return new ShortlistFilterResponse
            {
                ShortlistFilterId = entity.ShortlistFilterId,
                RequisitionId = entity.RequisitionId,
                FilterName = entity.FilterName,
                IsAutoShortlistEnabled = entity.IsAutoShortlistEnabled,
                RunOnSubmission = entity.RunOnSubmission,
                IsActive = entity.IsActive,
            };
        }
    }
}
