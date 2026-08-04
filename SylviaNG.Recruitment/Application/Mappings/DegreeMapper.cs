using SylviaNG.Recruitment.Application.Features.Degrees.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class DegreeMapper
    {
        public static Degree ToEntity(this DegreeCreateRequest request)
        {
            return new Degree
            {
                Name = request.Name,
                FullName = request.FullName,
                Position = request.Position,
            };
        }

        public static DegreeResponse ToResponse(this Degree entity)
        {
            return new DegreeResponse
            {
                DegreeId = entity.DegreeId,
                Name = entity.Name,
                FullName = entity.FullName,
                Position = entity.Position,
            };
        }
    }
}
