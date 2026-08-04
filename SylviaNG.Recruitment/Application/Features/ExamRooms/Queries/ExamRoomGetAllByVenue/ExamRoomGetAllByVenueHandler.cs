using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Queries.ExamRoomGetAllByVenue
{
    public class ExamRoomGetAllByVenueHandler : IRequestHandler<ExamRoomGetAllByVenueQuery, List<ExamRoomResponse>>
    {
        private readonly IExamRoomService _examRoomService;

        public ExamRoomGetAllByVenueHandler(IExamRoomService examRoomService)
        {
            _examRoomService = examRoomService;
        }

        public async Task<List<ExamRoomResponse>> Handle(ExamRoomGetAllByVenueQuery query, CancellationToken cancellationToken)
        {
            return await _examRoomService.GetAllByVenueIdAsync(query.ExamVenueId);
        }
    }
}
