using MediatR;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Queries.CareerContentGetById
{
    public class CareerContentGetByIdQuery : IRequest<CareerContentResponse>
    {
        public long CareerContentId { get; set; }

        public CareerContentGetByIdQuery(long careerContentId)
        {
            CareerContentId = careerContentId;
        }
    }
}
