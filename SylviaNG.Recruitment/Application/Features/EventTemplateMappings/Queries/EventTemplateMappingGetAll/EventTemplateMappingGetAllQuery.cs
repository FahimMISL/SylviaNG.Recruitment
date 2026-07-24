using MediatR;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Queries.EventTemplateMappingGetAll
{
    public class EventTemplateMappingGetAllQuery : IRequest<List<EventTemplateMappingResponse>>
    {
    }
}
