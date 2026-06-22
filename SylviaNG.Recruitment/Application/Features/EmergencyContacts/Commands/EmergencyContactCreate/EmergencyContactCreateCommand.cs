using MediatR;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactCreate
{
    public class EmergencyContactCreateCommand : IRequest<long>
    {
        public EmergencyContactCreateRequest Request { get; set; }

        public EmergencyContactCreateCommand(EmergencyContactCreateRequest request)
        {
            Request = request;
        }
    }
}
