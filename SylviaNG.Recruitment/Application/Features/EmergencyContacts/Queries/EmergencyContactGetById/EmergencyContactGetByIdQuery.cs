using MediatR;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Queries.EmergencyContactGetById
{
    public class EmergencyContactGetByIdQuery : IRequest<EmergencyContactResponse>
    {
        public long EmergencyContactId { get; set; }

        public EmergencyContactGetByIdQuery(long emergencyContactId)
        {
            EmergencyContactId = emergencyContactId;
        }
    }
}
