using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyUpdate
{
    public class RecruitmentAgencyUpdateHandler : IRequestHandler<RecruitmentAgencyUpdateCommand, Unit>
    {
        private readonly IRecruitmentAgencyService _service;

        public RecruitmentAgencyUpdateHandler(IRecruitmentAgencyService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RecruitmentAgencyUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.RecruitmentAgencyId, command.Request);
            return Unit.Value;
        }
    }
}
