using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardUpdate
{
    public class EducationBoardUpdateHandler : IRequestHandler<EducationBoardUpdateCommand, Unit>
    {
        private readonly IEducationBoardService _educationBoardService;

        public EducationBoardUpdateHandler(IEducationBoardService educationBoardService)
        {
            _educationBoardService = educationBoardService;
        }

        public async Task<Unit> Handle(EducationBoardUpdateCommand command, CancellationToken cancellationToken)
        {
            await _educationBoardService.UpdateAsync(command.EducationBoardId, command.Request);
            return Unit.Value;
        }
    }
}
