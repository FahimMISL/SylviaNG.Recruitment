using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Queries.DocumentAcceptanceGetAll
{
    public class DocumentAcceptanceGetAllQuery : IRequest<List<DocumentAcceptanceResponse>>
    {
    }
}
