using MediatR;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Models;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Queries.EducationBoardGetAll
{
    public class EducationBoardGetAllQuery : IRequest<List<EducationBoardResponse>>
    {
    }
}
