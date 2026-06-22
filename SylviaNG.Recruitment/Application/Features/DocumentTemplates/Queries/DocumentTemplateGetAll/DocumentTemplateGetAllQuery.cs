using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplateGetAll
{
    public class DocumentTemplateGetAllQuery : IRequest<List<DocumentTemplateResponse>>
    {
    }
}
