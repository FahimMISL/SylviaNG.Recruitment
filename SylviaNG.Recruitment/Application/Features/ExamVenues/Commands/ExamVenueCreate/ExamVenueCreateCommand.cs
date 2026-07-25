using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueCreate
{
    public class ExamVenueCreateCommand : IRequest<long>
    {
        public ExamVenueCreateRequest Request { get; set; }

        public ExamVenueCreateCommand(ExamVenueCreateRequest request)
        {
            Request = request;
        }
    }
}
