using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamRooms.Queries.ExamRoomGetById
{
    public class ExamRoomGetByIdHandler : IRequestHandler<ExamRoomGetByIdQuery, ExamRoomResponse>
    {
        private readonly IExamRoomService _examRoomService;

        public ExamRoomGetByIdHandler(IExamRoomService examRoomService)
        {
            _examRoomService = examRoomService;
        }

        public async Task<ExamRoomResponse> Handle(ExamRoomGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _examRoomService.GetByIdAsync(query.ExamRoomId);
        }
    }
}
