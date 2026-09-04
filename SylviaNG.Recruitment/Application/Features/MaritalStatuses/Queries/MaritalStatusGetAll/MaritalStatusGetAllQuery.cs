using MediatR;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Models;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Queries.MaritalStatusGetAll
{
    public class MaritalStatusGetAllQuery : IRequest<List<MaritalStatusResponse>>
    {
    }
}
