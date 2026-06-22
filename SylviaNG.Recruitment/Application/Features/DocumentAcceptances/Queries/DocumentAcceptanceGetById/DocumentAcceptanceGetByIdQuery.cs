using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Queries.DocumentAcceptanceGetById
{
    public class DocumentAcceptanceGetByIdQuery : IRequest<DocumentAcceptanceResponse>
    {
        public long DocumentAcceptanceId { get; set; }

        public DocumentAcceptanceGetByIdQuery(long documentAcceptanceId)
        {
            DocumentAcceptanceId = documentAcceptanceId;
        }
    }
}
