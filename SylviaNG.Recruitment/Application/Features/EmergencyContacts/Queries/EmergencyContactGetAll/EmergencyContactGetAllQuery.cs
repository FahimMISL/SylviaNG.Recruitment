using MediatR;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Queries.EmergencyContactGetAll
{
    public class EmergencyContactGetAllQuery : IRequest<List<EmergencyContactResponse>>
    {
    }
}
