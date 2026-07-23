using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Queries.InterviewRoomGetAllByVenue
{
    public class InterviewRoomGetAllByVenueHandler : IRequestHandler<InterviewRoomGetAllByVenueQuery, List<InterviewRoomResponse>>
    {
        private readonly IInterviewRoomService _interviewRoomService;

        public InterviewRoomGetAllByVenueHandler(IInterviewRoomService interviewRoomService)
        {
            _interviewRoomService = interviewRoomService;
        }

        public async Task<List<InterviewRoomResponse>> Handle(InterviewRoomGetAllByVenueQuery query, CancellationToken cancellationToken)
        {
            return await _interviewRoomService.GetAllByVenueIdAsync(query.InterviewVenueId);
        }
    }
}
