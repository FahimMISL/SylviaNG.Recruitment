using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardDelete
{
    public class EducationBoardDeleteHandler : IRequestHandler<EducationBoardDeleteCommand, Unit>
    {
        private readonly IEducationBoardService _educationBoardService;

        public EducationBoardDeleteHandler(IEducationBoardService educationBoardService)
        {
            _educationBoardService = educationBoardService;
        }

        public async Task<Unit> Handle(EducationBoardDeleteCommand command, CancellationToken cancellationToken)
        {
            await _educationBoardService.DeleteAsync(command.EducationBoardId);
            return Unit.Value;
        }
    }
}
