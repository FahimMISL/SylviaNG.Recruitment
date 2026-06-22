using MediatR;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentCreate
{
    public class CareerContentCreateCommand : IRequest<long>
    {
        public CareerContentCreateRequest Request { get; set; }

        public CareerContentCreateCommand(CareerContentCreateRequest request)
        {
            Request = request;
        }
    }
}
