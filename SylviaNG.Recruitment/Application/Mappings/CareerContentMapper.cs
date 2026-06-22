using SylviaNG.Recruitment.Application.Features.CareerContents.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CareerContentMapper
    {
        public static CareerContent ToEntity(this CareerContentCreateRequest request)
        {
            return new CareerContent
            {
                ContentType = request.ContentType,
                Title = request.Title,
                Body = request.Body,
                MediaUrl = request.MediaUrl,
                SortOrder = request.SortOrder,
                IsPublished = request.IsPublished
            };
        }

        public static void ApplyUpdate(this CareerContent entity, CareerContentUpdateRequest request)
        {
            if (request.ContentType.HasValue) entity.ContentType = request.ContentType.Value;
            if (request.Title is not null) entity.Title = request.Title;
            if (request.Body is not null) entity.Body = request.Body;
            if (request.MediaUrl is not null) entity.MediaUrl = request.MediaUrl;
            if (request.SortOrder.HasValue) entity.SortOrder = request.SortOrder.Value;
            if (request.IsPublished.HasValue) entity.IsPublished = request.IsPublished.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static CareerContentResponse ToResponse(this CareerContent entity)
        {
            return new CareerContentResponse
            {
                CareerContentId = entity.CareerContentId,
                ContentType = entity.ContentType,
                Title = entity.Title,
                Body = entity.Body,
                MediaUrl = entity.MediaUrl,
                SortOrder = entity.SortOrder,
                IsPublished = entity.IsPublished,
                IsActive = entity.IsActive
            };
        }
    }
}
