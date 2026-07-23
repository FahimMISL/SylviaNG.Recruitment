using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewRooms.Commands.InterviewRoomUpdate
{
    public class InterviewRoomUpdateValidator : AbstractValidator<InterviewRoomUpdateCommand>
    {
        public InterviewRoomUpdateValidator()
        {
            RuleFor(x => x.Request.RoomName)
                .NotEmpty().WithMessage("Room name is required.")
                .MaximumLength(200).WithMessage("Room name must not exceed 200 characters.");

            RuleFor(x => x.Request.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.");
        }
    }
}
