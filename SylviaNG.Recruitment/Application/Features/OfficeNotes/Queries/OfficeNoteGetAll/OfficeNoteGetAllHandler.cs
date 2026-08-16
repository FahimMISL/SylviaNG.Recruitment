using MediatR;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Queries.OfficeNoteGetAll
{
    public class OfficeNoteGetAllHandler : IRequestHandler<OfficeNoteGetAllQuery, List<OfficeNoteResponse>>
    {
        private readonly IOfficeNoteService _officeNoteService;

        public OfficeNoteGetAllHandler(IOfficeNoteService officeNoteService)
        {
            _officeNoteService = officeNoteService;
        }

        public async Task<List<OfficeNoteResponse>> Handle(OfficeNoteGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _officeNoteService.GetAllAsync(query.JobApplicationId);
        }
    }
}
