using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Commands.MajorSubjectUniversityCreate
{
    public class MajorSubjectUniversityCreateHandler : IRequestHandler<MajorSubjectUniversityCreateCommand, long>
    {
        private readonly IMajorSubjectUniversityService _majorSubjectUniversityService;

        public MajorSubjectUniversityCreateHandler(IMajorSubjectUniversityService majorSubjectUniversityService)
        {
            _majorSubjectUniversityService = majorSubjectUniversityService;
        }

        public async Task<long> Handle(MajorSubjectUniversityCreateCommand command, CancellationToken cancellationToken)
        {
            return await _majorSubjectUniversityService.CreateAsync(command.Request);
        }
    }
}
