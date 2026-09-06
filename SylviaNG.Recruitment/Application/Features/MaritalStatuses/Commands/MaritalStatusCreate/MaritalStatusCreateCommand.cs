using MediatR;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Models;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusCreate
{
    public class MaritalStatusCreateCommand : IRequest<long>
    {
        public MaritalStatusCreateRequest Request { get; set; }

        public MaritalStatusCreateCommand(MaritalStatusCreateRequest request)
        {
            Request = request;
        }
    }
}
