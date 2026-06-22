using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentDelete
{
    public class CareerContentDeleteHandler : IRequestHandler<CareerContentDeleteCommand, Unit>
    {
        private readonly ICareerContentService _service;

        public CareerContentDeleteHandler(ICareerContentService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CareerContentDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.CareerContentId);
            return Unit.Value;
        }
    }
}
