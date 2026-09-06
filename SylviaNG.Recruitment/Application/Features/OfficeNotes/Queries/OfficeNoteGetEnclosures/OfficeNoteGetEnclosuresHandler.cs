using MediatR;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Queries.OfficeNoteGetEnclosures
{
    public class OfficeNoteGetEnclosuresHandler : IRequestHandler<OfficeNoteGetEnclosuresQuery, OfficeNoteEnclosuresResponse>
    {
        private readonly IOfficeNoteService _officeNoteService;

        public OfficeNoteGetEnclosuresHandler(IOfficeNoteService officeNoteService)
        {
            _officeNoteService = officeNoteService;
        }

        public async Task<OfficeNoteEnclosuresResponse> Handle(OfficeNoteGetEnclosuresQuery query, CancellationToken cancellationToken)
        {
            return await _officeNoteService.GetEnclosuresAsync(query.JobApplicationId);
        }
    }
}
