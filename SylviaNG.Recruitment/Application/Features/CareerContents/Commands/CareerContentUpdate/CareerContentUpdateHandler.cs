using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentUpdate
{
    public class CareerContentUpdateHandler : IRequestHandler<CareerContentUpdateCommand, Unit>
    {
        private readonly ICareerContentService _service;

        public CareerContentUpdateHandler(ICareerContentService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CareerContentUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.CareerContentId, command.Request);
            return Unit.Value;
        }
    }
}
