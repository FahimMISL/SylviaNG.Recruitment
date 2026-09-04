using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class MajorSubjectUniversityMapper
    {
        public static MajorSubjectUniversity ToEntity(this MajorSubjectUniversityCreateRequest request)
        {
            return new MajorSubjectUniversity
            {
                Name = request.Name,
            };
        }

        public static MajorSubjectUniversityResponse ToResponse(this MajorSubjectUniversity entity)
        {
            return new MajorSubjectUniversityResponse
            {
                MajorSubjectUniversityId = entity.MajorSubjectUniversityId,
                Name = entity.Name,
            };
        }
    }
}
