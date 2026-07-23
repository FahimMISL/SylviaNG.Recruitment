using MediatR;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Models;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardUpdate
{
    public class EducationBoardUpdateCommand : IRequest<Unit>
    {
        public long EducationBoardId { get; set; }
        public EducationBoardUpdateRequest Request { get; set; }

        public EducationBoardUpdateCommand(long educationBoardId, EducationBoardUpdateRequest request)
        {
            EducationBoardId = educationBoardId;
            Request = request;
        }
    }
}
