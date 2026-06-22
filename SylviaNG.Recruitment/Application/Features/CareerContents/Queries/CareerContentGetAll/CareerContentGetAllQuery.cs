using MediatR;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Queries.CareerContentGetAll
{
    public class CareerContentGetAllQuery : IRequest<List<CareerContentResponse>>
    {
    }
}
