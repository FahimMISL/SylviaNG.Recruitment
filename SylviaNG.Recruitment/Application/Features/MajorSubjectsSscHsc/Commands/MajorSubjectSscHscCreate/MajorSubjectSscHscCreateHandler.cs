using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Commands.MajorSubjectSscHscCreate
{
    public class MajorSubjectSscHscCreateHandler : IRequestHandler<MajorSubjectSscHscCreateCommand, long>
    {
        private readonly IMajorSubjectSscHscService _majorSubjectSscHscService;

        public MajorSubjectSscHscCreateHandler(IMajorSubjectSscHscService majorSubjectSscHscService)
        {
            _majorSubjectSscHscService = majorSubjectSscHscService;
        }

        public async Task<long> Handle(MajorSubjectSscHscCreateCommand command, CancellationToken cancellationToken)
        {
            return await _majorSubjectSscHscService.CreateAsync(command.Request);
        }
    }
}
