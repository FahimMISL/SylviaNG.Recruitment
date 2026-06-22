using SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class RequisitionTemplateMapper
    {
        public static RequisitionTemplate ToEntity(this RequisitionTemplateCreateRequest request)
        {
            return new RequisitionTemplate
            {
                TemplateName = request.TemplateName,
                Description = request.Description,
                StageConfigJson = request.StageConfigJson,
            };
        }

        public static void ApplyUpdate(this RequisitionTemplate entity, RequisitionTemplateUpdateRequest request)
        {
            if (request.TemplateName is not null) entity.TemplateName = request.TemplateName;
            if (request.Description is not null) entity.Description = request.Description;
            if (request.StageConfigJson is not null) entity.StageConfigJson = request.StageConfigJson;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static RequisitionTemplateResponse ToResponse(this RequisitionTemplate entity)
        {
            return new RequisitionTemplateResponse
            {
                RequisitionTemplateId = entity.RequisitionTemplateId,
                TemplateName = entity.TemplateName,
                Description = entity.Description,
                StageConfigJson = entity.StageConfigJson,
                IsActive = entity.IsActive,
            };
        }
    }
}
