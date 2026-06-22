using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyDelete
{
    public class RecruitmentAgencyDeleteHandler : IRequestHandler<RecruitmentAgencyDeleteCommand, Unit>
    {
        private readonly IRecruitmentAgencyService _service;

        public RecruitmentAgencyDeleteHandler(IRecruitmentAgencyService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RecruitmentAgencyDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.RecruitmentAgencyId);
            return Unit.Value;
        }
    }
}
