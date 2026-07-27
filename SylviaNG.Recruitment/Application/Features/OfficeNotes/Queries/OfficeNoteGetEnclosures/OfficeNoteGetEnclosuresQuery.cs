using MediatR;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;

namespace SylviaNG.Recruitment.Application.Features.OfficeNotes.Queries.OfficeNoteGetEnclosures
{
    public class OfficeNoteGetEnclosuresQuery : IRequest<OfficeNoteEnclosuresResponse>
    {
        public long JobApplicationId { get; set; }

        public OfficeNoteGetEnclosuresQuery(long jobApplicationId)
        {
            JobApplicationId = jobApplicationId;
        }
    }
}
