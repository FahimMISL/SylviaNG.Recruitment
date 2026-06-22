using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class RequisitionStageConfigMapper
    {
        public static RequisitionStageConfig ToEntity(this RequisitionStageConfigCreateRequest request)
        {
            return new RequisitionStageConfig
            {
                RequisitionId = request.RequisitionId,
                StageName = request.StageName,
                StageOrder = request.StageOrder,
                IsMandatory = request.IsMandatory,
            };
        }

        public static void ApplyUpdate(this RequisitionStageConfig entity, RequisitionStageConfigUpdateRequest request)
        {
            if (request.StageName is not null) entity.StageName = request.StageName;
            if (request.StageOrder.HasValue) entity.StageOrder = request.StageOrder.Value;
            if (request.IsMandatory.HasValue) entity.IsMandatory = request.IsMandatory.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static RequisitionStageConfigResponse ToResponse(this RequisitionStageConfig entity)
        {
            return new RequisitionStageConfigResponse
            {
                RequisitionStageConfigId = entity.RequisitionStageConfigId,
                RequisitionId = entity.RequisitionId,
                StageName = entity.StageName,
                StageOrder = entity.StageOrder,
                IsMandatory = entity.IsMandatory,
                IsActive = entity.IsActive,
            };
        }
    }
}
