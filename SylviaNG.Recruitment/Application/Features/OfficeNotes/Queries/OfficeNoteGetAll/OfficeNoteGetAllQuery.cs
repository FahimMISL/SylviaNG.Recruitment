using MediatR;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Queries.OfficeNoteGetAll
{
    public class OfficeNoteGetAllQuery : IRequest<List<OfficeNoteResponse>>
    {
        public long? JobApplicationId { get; set; }

        public OfficeNoteGetAllQuery(long? jobApplicationId)
        {
            JobApplicationId = jobApplicationId;
        }
    }
}
