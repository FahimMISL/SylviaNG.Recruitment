using MediatR;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Models;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardCreate
{
    public class EducationBoardCreateCommand : IRequest<long>
    {
        public EducationBoardCreateRequest Request { get; set; }

        public EducationBoardCreateCommand(EducationBoardCreateRequest request)
        {
            Request = request;
        }
    }
}
