using SylviaNG.Recruitment.Application.Features.BloodGroups.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class BloodGroupMapper
    {
        public static BloodGroup ToEntity(this BloodGroupCreateRequest request)
        {
            return new BloodGroup
            {
                Name = request.Name,
            };
        }

        public static BloodGroupResponse ToResponse(this BloodGroup entity)
        {
            return new BloodGroupResponse
            {
                BloodGroupId = entity.BloodGroupId,
                Name = entity.Name,
            };
        }
    }
}
