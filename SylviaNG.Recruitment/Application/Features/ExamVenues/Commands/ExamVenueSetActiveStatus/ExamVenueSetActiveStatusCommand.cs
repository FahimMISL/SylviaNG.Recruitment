using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamVenues.Commands.ExamVenueSetActiveStatus
{
    public class ExamVenueSetActiveStatusCommand : IRequest
    {
        public long ExamVenueId { get; set; }
        public bool IsActive { get; set; }

        public ExamVenueSetActiveStatusCommand(long examVenueId, bool isActive)
        {
            ExamVenueId = examVenueId;
            IsActive = isActive;
        }
    }
}
