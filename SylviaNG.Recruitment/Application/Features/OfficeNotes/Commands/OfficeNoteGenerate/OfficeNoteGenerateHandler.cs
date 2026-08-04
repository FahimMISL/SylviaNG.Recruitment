using MediatR;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Commands.OfficeNoteGenerate
{
    public class OfficeNoteGenerateHandler : IRequestHandler<OfficeNoteGenerateCommand, OfficeNoteResponse>
    {
        private readonly IOfficeNoteService _officeNoteService;

        public OfficeNoteGenerateHandler(IOfficeNoteService officeNoteService)
        {
            _officeNoteService = officeNoteService;
        }

        public async Task<OfficeNoteResponse> Handle(OfficeNoteGenerateCommand command, CancellationToken cancellationToken)
        {
            return await _officeNoteService.GenerateAsync(command.Request);
        }
    }
}
