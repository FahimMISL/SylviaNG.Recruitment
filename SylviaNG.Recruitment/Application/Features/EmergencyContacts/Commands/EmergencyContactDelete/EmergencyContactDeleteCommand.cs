using MediatR;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactDelete
{
    public class EmergencyContactDeleteCommand : IRequest<Unit>
    {
        public long EmergencyContactId { get; set; }

        public EmergencyContactDeleteCommand(long emergencyContactId)
        {
            EmergencyContactId = emergencyContactId;
        }
    }
}
