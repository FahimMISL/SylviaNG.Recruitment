using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class MaritalStatusMapper
    {
        public static MaritalStatus ToEntity(this MaritalStatusCreateRequest request)
        {
            return new MaritalStatus
            {
                Name = request.Name,
            };
        }

        public static MaritalStatusResponse ToResponse(this MaritalStatus entity)
        {
            return new MaritalStatusResponse
            {
                MaritalStatusId = entity.MaritalStatusId,
                Name = entity.Name,
            };
        }
    }
}
