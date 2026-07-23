using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamEnrollmentReassignSeat
{
    public class ExamEnrollmentReassignSeatCommand : IRequest
    {
        public long ExamEnrollmentId { get; set; }
        public long ExamRoomId { get; set; }
        public string SeatNumber { get; set; }

        public ExamEnrollmentReassignSeatCommand(long examEnrollmentId, long examRoomId, string seatNumber)
        {
            ExamEnrollmentId = examEnrollmentId;
            ExamRoomId = examRoomId;
            SeatNumber = seatNumber;
        }
    }
}
