using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Queries.DocumentTemplateVersionGetAll
{
    public class DocumentTemplateVersionGetAllQuery : IRequest<List<DocumentTemplateVersionResponse>>
    {
    }
}
