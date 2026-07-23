using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomCreate
{
    public class InterviewRoomCreateHandler : IRequestHandler<InterviewRoomCreateCommand, long>
    {
        private readonly IInterviewRoomService _interviewRoomService;

        public InterviewRoomCreateHandler(IInterviewRoomService interviewRoomService)
        {
            _interviewRoomService = interviewRoomService;
        }

        public async Task<long> Handle(InterviewRoomCreateCommand command, CancellationToken cancellationToken)
        {
            return await _interviewRoomService.CreateAsync(command.InterviewVenueId, command.Request);
        }
    }
}
