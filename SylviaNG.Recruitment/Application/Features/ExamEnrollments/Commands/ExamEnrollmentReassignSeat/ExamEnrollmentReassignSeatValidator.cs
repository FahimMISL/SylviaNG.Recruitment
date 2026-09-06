using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamEnrollmentReassignSeat
{
    public class ExamEnrollmentReassignSeatValidator : AbstractValidator<ExamEnrollmentReassignSeatCommand>
    {
        public ExamEnrollmentReassignSeatValidator()
        {
            RuleFor(x => x.ExamEnrollmentId)
                .GreaterThan(0).WithMessage("ExamEnrollmentId is required.");

            RuleFor(x => x.ExamRoomId)
                .GreaterThan(0).WithMessage("ExamRoomId is required.");

            RuleFor(x => x.SeatNumber)
                .NotEmpty().WithMessage("SeatNumber is required.")
                .MaximumLength(50);
        }
    }
}
