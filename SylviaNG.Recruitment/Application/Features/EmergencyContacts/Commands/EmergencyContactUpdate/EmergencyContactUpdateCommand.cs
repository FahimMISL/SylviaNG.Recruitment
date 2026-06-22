using MediatR;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactUpdate
{
    public class EmergencyContactUpdateCommand : IRequest<Unit>
    {
        public long EmergencyContactId { get; set; }
        public EmergencyContactUpdateRequest Request { get; set; }

        public EmergencyContactUpdateCommand(long emergencyContactId, EmergencyContactUpdateRequest request)
        {
            EmergencyContactId = emergencyContactId;
            Request = request;
        }
    }
}
