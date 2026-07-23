using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscDelete
{
    public class MajorSubjectSscHscDeleteHandler : IRequestHandler<MajorSubjectSscHscDeleteCommand, Unit>
    {
        private readonly IMajorSubjectSscHscService _majorSubjectSscHscService;

        public MajorSubjectSscHscDeleteHandler(IMajorSubjectSscHscService majorSubjectSscHscService)
        {
            _majorSubjectSscHscService = majorSubjectSscHscService;
        }

        public async Task<Unit> Handle(MajorSubjectSscHscDeleteCommand command, CancellationToken cancellationToken)
        {
            await _majorSubjectSscHscService.DeleteAsync(command.MajorSubjectSscHscId);
            return Unit.Value;
        }
    }
}
