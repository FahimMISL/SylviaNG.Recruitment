using MediatR;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Queries.OfficeNoteGetById
{
    public class OfficeNoteGetByIdHandler : IRequestHandler<OfficeNoteGetByIdQuery, OfficeNoteResponse>
    {
        private readonly IOfficeNoteService _officeNoteService;

        public OfficeNoteGetByIdHandler(IOfficeNoteService officeNoteService)
        {
            _officeNoteService = officeNoteService;
        }

        public async Task<OfficeNoteResponse> Handle(OfficeNoteGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _officeNoteService.GetByIdAsync(query.OfficeNoteId);
        }
    }
}
