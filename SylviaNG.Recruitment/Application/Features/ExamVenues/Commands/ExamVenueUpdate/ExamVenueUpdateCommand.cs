using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueUpdate
{
    public class ExamVenueUpdateCommand : IRequest
    {
        public long ExamVenueId { get; set; }
        public ExamVenueUpdateRequest Request { get; set; }

        public ExamVenueUpdateCommand(long examVenueId, ExamVenueUpdateRequest request)
        {
            ExamVenueId = examVenueId;
            Request = request;
        }
    }
}
