using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityDelete
{
    public class MajorSubjectUniversityDeleteHandler : IRequestHandler<MajorSubjectUniversityDeleteCommand, Unit>
    {
        private readonly IMajorSubjectUniversityService _majorSubjectUniversityService;

        public MajorSubjectUniversityDeleteHandler(IMajorSubjectUniversityService majorSubjectUniversityService)
        {
            _majorSubjectUniversityService = majorSubjectUniversityService;
        }

        public async Task<Unit> Handle(MajorSubjectUniversityDeleteCommand command, CancellationToken cancellationToken)
        {
            await _majorSubjectUniversityService.DeleteAsync(command.MajorSubjectUniversityId);
            return Unit.Value;
        }
    }
}
