using MediatR;
using SylviaNG.Recruitment.Application.Features.EducationBoards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Queries.EducationBoardGetAll
{
    public class EducationBoardGetAllHandler : IRequestHandler<EducationBoardGetAllQuery, List<EducationBoardResponse>>
    {
        private readonly IEducationBoardService _educationBoardService;

        public EducationBoardGetAllHandler(IEducationBoardService educationBoardService)
        {
            _educationBoardService = educationBoardService;
        }

        public async Task<List<EducationBoardResponse>> Handle(EducationBoardGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _educationBoardService.GetAllAsync();
        }
    }
}
