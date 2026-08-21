using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Queries.InterviewRoomGetById
{
    public class InterviewRoomGetByIdHandler : IRequestHandler<InterviewRoomGetByIdQuery, InterviewRoomResponse>
    {
        private readonly IInterviewRoomService _interviewRoomService;

        public InterviewRoomGetByIdHandler(IInterviewRoomService interviewRoomService)
        {
            _interviewRoomService = interviewRoomService;
        }

        public async Task<InterviewRoomResponse> Handle(InterviewRoomGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _interviewRoomService.GetByIdAsync(query.InterviewRoomId);
        }
    }
}
