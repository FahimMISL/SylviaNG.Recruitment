using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Queries.ExamVenueGetById
{
    public class ExamVenueGetByIdQuery : IRequest<ExamVenueResponse>
    {
        public long ExamVenueId { get; set; }

        public ExamVenueGetByIdQuery(long examVenueId)
        {
            ExamVenueId = examVenueId;
        }
    }
}
