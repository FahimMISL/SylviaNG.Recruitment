using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ShortlistFilterCriteriaMapper
    {
        public static ShortlistFilterCriteria ToEntity(this ShortlistFilterCriteriaCreateRequest request)
        {
            return new ShortlistFilterCriteria
            {
                ShortlistFilterId = request.ShortlistFilterId,
                FieldName = request.FieldName,
                Operator = request.Operator,
                Value = request.Value,
                IsHardFilter = request.IsHardFilter,
                LayerOrder = request.LayerOrder,
            };
        }

        public static void ApplyUpdate(this ShortlistFilterCriteria entity, ShortlistFilterCriteriaUpdateRequest request)
        {
            if (request.ShortlistFilterId.HasValue) entity.ShortlistFilterId = request.ShortlistFilterId.Value;
            if (request.FieldName is not null) entity.FieldName = request.FieldName;
            if (request.Operator is not null) entity.Operator = request.Operator;
            if (request.Value is not null) entity.Value = request.Value;
            if (request.IsHardFilter.HasValue) entity.IsHardFilter = request.IsHardFilter.Value;
            if (request.LayerOrder.HasValue) entity.LayerOrder = request.LayerOrder.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ShortlistFilterCriteriaResponse ToResponse(this ShortlistFilterCriteria entity)
        {
            return new ShortlistFilterCriteriaResponse
            {
                ShortlistFilterCriteriaId = entity.ShortlistFilterCriteriaId,
                ShortlistFilterId = entity.ShortlistFilterId,
                FieldName = entity.FieldName,
                Operator = entity.Operator,
                Value = entity.Value,
                IsHardFilter = entity.IsHardFilter,
                LayerOrder = entity.LayerOrder,
                IsActive = entity.IsActive,
            };
        }
    }
}
