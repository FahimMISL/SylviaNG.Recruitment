using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentCreate
{
    public class CareerContentCreateHandler : IRequestHandler<CareerContentCreateCommand, long>
    {
        private readonly ICareerContentService _service;

        public CareerContentCreateHandler(ICareerContentService service)
        {
            _service = service;
        }

        public async Task<long> Handle(CareerContentCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
