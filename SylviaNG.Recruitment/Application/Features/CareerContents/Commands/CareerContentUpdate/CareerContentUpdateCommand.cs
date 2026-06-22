using MediatR;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentUpdate
{
    public class CareerContentUpdateCommand : IRequest<Unit>
    {
        public long CareerContentId { get; set; }
        public CareerContentUpdateRequest Request { get; set; }

        public CareerContentUpdateCommand(long careerContentId, CareerContentUpdateRequest request)
        {
            CareerContentId = careerContentId;
            Request = request;
        }
    }
}
