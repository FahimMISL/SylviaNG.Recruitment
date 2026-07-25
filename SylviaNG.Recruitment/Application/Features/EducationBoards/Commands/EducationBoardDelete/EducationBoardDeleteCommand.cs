using MediatR;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardDelete
{
    public class EducationBoardDeleteCommand : IRequest<Unit>
    {
        public long EducationBoardId { get; set; }

        public EducationBoardDeleteCommand(long educationBoardId)
        {
            EducationBoardId = educationBoardId;
        }
    }
}
