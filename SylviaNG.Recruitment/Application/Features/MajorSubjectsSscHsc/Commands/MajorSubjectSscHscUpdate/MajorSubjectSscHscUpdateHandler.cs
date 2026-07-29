using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscUpdate
{
    public class MajorSubjectSscHscUpdateHandler : IRequestHandler<MajorSubjectSscHscUpdateCommand, Unit>
    {
        private readonly IMajorSubjectSscHscService _majorSubjectSscHscService;

        public MajorSubjectSscHscUpdateHandler(IMajorSubjectSscHscService majorSubjectSscHscService)
        {
            _majorSubjectSscHscService = majorSubjectSscHscService;
        }

        public async Task<Unit> Handle(MajorSubjectSscHscUpdateCommand command, CancellationToken cancellationToken)
        {
            await _majorSubjectSscHscService.UpdateAsync(command.MajorSubjectSscHscId, command.Request);
            return Unit.Value;
        }
    }
}
