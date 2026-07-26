using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamEnrollmentReassignSeat
{
    public class ExamEnrollmentReassignSeatHandler : IRequestHandler<ExamEnrollmentReassignSeatCommand>
    {
        private readonly IExamEnrollmentService _examEnrollmentService;

        public ExamEnrollmentReassignSeatHandler(IExamEnrollmentService examEnrollmentService)
        {
            _examEnrollmentService = examEnrollmentService;
        }

        public async Task Handle(ExamEnrollmentReassignSeatCommand command, CancellationToken cancellationToken)
        {
            await _examEnrollmentService.ReassignSeatAsync(command.ExamEnrollmentId, command.ExamRoomId, command.SeatNumber);
        }
    }
}
