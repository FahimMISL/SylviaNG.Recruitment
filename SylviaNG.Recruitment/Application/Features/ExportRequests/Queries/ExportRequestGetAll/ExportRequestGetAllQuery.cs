using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetAll
{
    public class ExportRequestGetAllQuery : IRequest<List<ExportRequestResponse>>
    {
    }
}
