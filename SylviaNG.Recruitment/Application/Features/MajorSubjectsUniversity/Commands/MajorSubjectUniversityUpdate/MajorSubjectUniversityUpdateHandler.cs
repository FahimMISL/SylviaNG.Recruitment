using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityUpdate
{
    public class MajorSubjectUniversityUpdateHandler : IRequestHandler<MajorSubjectUniversityUpdateCommand, Unit>
    {
        private readonly IMajorSubjectUniversityService _majorSubjectUniversityService;

        public MajorSubjectUniversityUpdateHandler(IMajorSubjectUniversityService majorSubjectUniversityService)
        {
            _majorSubjectUniversityService = majorSubjectUniversityService;
        }

        public async Task<Unit> Handle(MajorSubjectUniversityUpdateCommand command, CancellationToken cancellationToken)
        {
            await _majorSubjectUniversityService.UpdateAsync(command.MajorSubjectUniversityId, command.Request);
            return Unit.Value;
        }
    }
}
