using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentDelete
{
    public class CareerContentDeleteCommand : IRequest<Unit>
    {
        public long CareerContentId { get; set; }

        public CareerContentDeleteCommand(long careerContentId)
        {
            CareerContentId = careerContentId;
        }
    }
}
