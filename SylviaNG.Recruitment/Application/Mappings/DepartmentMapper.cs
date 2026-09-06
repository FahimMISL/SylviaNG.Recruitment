using SylviaNG.Recruitment.Application.Features.Departments.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class DepartmentMapper
    {
        public static Department ToEntity(this DepartmentCreateRequest request)
        {
            return new Department
            {
                Name = request.Name,
            };
        }

        public static DepartmentResponse ToResponse(this Department entity)
        {
            return new DepartmentResponse
            {
                DepartmentId = entity.DepartmentId,
                Name = entity.Name,
            };
        }
    }
}
