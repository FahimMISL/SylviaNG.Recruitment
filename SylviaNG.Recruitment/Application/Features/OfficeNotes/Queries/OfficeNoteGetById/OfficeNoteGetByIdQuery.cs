using MediatR;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Queries.OfficeNoteGetById
{
    public class OfficeNoteGetByIdQuery : IRequest<OfficeNoteResponse>
    {
        public long OfficeNoteId { get; set; }

        public OfficeNoteGetByIdQuery(long officeNoteId)
        {
            OfficeNoteId = officeNoteId;
        }
    }
}
