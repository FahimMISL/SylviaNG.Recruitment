using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceCreate
{
    public class DocumentAcceptanceCreateCommand : IRequest<long>
    {
        public DocumentAcceptanceCreateRequest Request { get; set; }

        public DocumentAcceptanceCreateCommand(DocumentAcceptanceCreateRequest request)
        {
            Request = request;
        }
    }
}
