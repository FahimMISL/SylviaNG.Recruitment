using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletGenerate
{
    public class JoiningBookletGenerateHandler : IRequestHandler<JoiningBookletGenerateCommand, JoiningBookletResponse>
    {
        private readonly IJoiningBookletService _joiningBookletService;

        public JoiningBookletGenerateHandler(IJoiningBookletService joiningBookletService)
        {
            _joiningBookletService = joiningBookletService;
        }

        public async Task<JoiningBookletResponse> Handle(JoiningBookletGenerateCommand command, CancellationToken cancellationToken)
        {
            return await _joiningBookletService.GenerateAsync(command.Request);
        }
    }
}
