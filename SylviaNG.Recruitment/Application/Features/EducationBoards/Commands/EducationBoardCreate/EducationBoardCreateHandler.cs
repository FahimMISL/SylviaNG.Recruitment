using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EducationBoards.Commands.EducationBoardCreate
{
    public class EducationBoardCreateHandler : IRequestHandler<EducationBoardCreateCommand, long>
    {
        private readonly IEducationBoardService _educationBoardService;

        public EducationBoardCreateHandler(IEducationBoardService educationBoardService)
        {
            _educationBoardService = educationBoardService;
        }

        public async Task<long> Handle(EducationBoardCreateCommand command, CancellationToken cancellationToken)
        {
            return await _educationBoardService.CreateAsync(command.Request);
        }
    }
}
