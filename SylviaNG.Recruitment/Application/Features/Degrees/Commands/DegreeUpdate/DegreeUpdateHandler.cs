using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeUpdate
{
    public class DegreeUpdateHandler : IRequestHandler<DegreeUpdateCommand, Unit>
    {
        private readonly IDegreeService _degreeService;

        public DegreeUpdateHandler(IDegreeService degreeService)
        {
            _degreeService = degreeService;
        }

        public async Task<Unit> Handle(DegreeUpdateCommand command, CancellationToken cancellationToken)
        {
            await _degreeService.UpdateAsync(command.DegreeId, command.Request);
            return Unit.Value;
        }
    }
}
