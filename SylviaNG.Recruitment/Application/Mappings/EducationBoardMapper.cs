using SylviaNG.Recruitment.Application.Features.EducationBoards.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class EducationBoardMapper
    {
        public static EducationBoard ToEntity(this EducationBoardCreateRequest request)
        {
            return new EducationBoard
            {
                Code = request.Code,
                Name = request.Name,
            };
        }

        public static EducationBoardResponse ToResponse(this EducationBoard entity)
        {
            return new EducationBoardResponse
            {
                EducationBoardId = entity.EducationBoardId,
                Code = entity.Code,
                Name = entity.Name,
            };
        }
    }
}
