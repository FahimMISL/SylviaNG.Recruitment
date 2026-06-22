using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class IntegrationLogMapper
    {
        public static IntegrationLog ToEntity(this IntegrationLogCreateRequest request)
        {
            return new IntegrationLog
            {
                IntegrationType = request.IntegrationType,
                OperationName = request.OperationName,
                RequestPayloadJson = request.RequestPayloadJson,
                ResponsePayloadJson = request.ResponsePayloadJson,
                HttpStatusCode = request.HttpStatusCode,
                IsSuccess = request.IsSuccess,
                ErrorMessage = request.ErrorMessage,
                ExecutedAt = request.ExecutedAt,
                DurationMs = request.DurationMs,
            };
        }

        public static void ApplyUpdate(this IntegrationLog entity, IntegrationLogUpdateRequest request)
        {
            if (request.IntegrationType.HasValue) entity.IntegrationType = request.IntegrationType.Value;
            if (request.OperationName is not null) entity.OperationName = request.OperationName;
            if (request.RequestPayloadJson is not null) entity.RequestPayloadJson = request.RequestPayloadJson;
            if (request.ResponsePayloadJson is not null) entity.ResponsePayloadJson = request.ResponsePayloadJson;
            if (request.HttpStatusCode.HasValue) entity.HttpStatusCode = request.HttpStatusCode.Value;
            if (request.IsSuccess.HasValue) entity.IsSuccess = request.IsSuccess.Value;
            if (request.ErrorMessage is not null) entity.ErrorMessage = request.ErrorMessage;
            if (request.ExecutedAt.HasValue) entity.ExecutedAt = request.ExecutedAt.Value;
            if (request.DurationMs.HasValue) entity.DurationMs = request.DurationMs.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static IntegrationLogResponse ToResponse(this IntegrationLog entity)
        {
            return new IntegrationLogResponse
            {
                IntegrationLogId = entity.IntegrationLogId,
                IntegrationType = entity.IntegrationType,
                OperationName = entity.OperationName,
                RequestPayloadJson = entity.RequestPayloadJson,
                ResponsePayloadJson = entity.ResponsePayloadJson,
                HttpStatusCode = entity.HttpStatusCode,
                IsSuccess = entity.IsSuccess,
                ErrorMessage = entity.ErrorMessage,
                ExecutedAt = entity.ExecutedAt,
                DurationMs = entity.DurationMs,
                IsActive = entity.IsActive,
            };
        }
    }
}
