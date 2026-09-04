using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class MajorSubjectSscHscMapper
    {
        public static MajorSubjectSscHsc ToEntity(this MajorSubjectSscHscCreateRequest request)
        {
            return new MajorSubjectSscHsc
            {
                Name = request.Name,
            };
        }

        public static MajorSubjectSscHscResponse ToResponse(this MajorSubjectSscHsc entity)
        {
            return new MajorSubjectSscHscResponse
            {
                MajorSubjectSscHscId = entity.MajorSubjectSscHscId,
                Name = entity.Name,
            };
        }
    }
}
