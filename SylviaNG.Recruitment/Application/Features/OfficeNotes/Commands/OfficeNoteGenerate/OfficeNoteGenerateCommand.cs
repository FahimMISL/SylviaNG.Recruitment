using MediatR;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Commands.OfficeNoteGenerate
{
    public class OfficeNoteGenerateCommand : IRequest<OfficeNoteResponse>
    {
        public OfficeNoteGenerateRequest Request { get; set; }

        public OfficeNoteGenerateCommand(OfficeNoteGenerateRequest request)
        {
            Request = request;
        }
    }
}
