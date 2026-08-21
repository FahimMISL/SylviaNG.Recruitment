using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletBulkGenerate
{
    public class JoiningBookletBulkGenerateHandler : IRequestHandler<JoiningBookletBulkGenerateCommand, JoiningBookletBulkGenerateResponse>
    {
        private readonly IJoiningBookletService _joiningBookletService;

        public JoiningBookletBulkGenerateHandler(IJoiningBookletService joiningBookletService)
        {
            _joiningBookletService = joiningBookletService;
        }

        public async Task<JoiningBookletBulkGenerateResponse> Handle(JoiningBookletBulkGenerateCommand command, CancellationToken cancellationToken)
        {
            return await _joiningBookletService.BulkGenerateAsync(command.Request);
        }
    }
}
