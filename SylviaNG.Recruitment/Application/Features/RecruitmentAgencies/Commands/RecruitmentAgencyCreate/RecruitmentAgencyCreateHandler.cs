using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Commands.RecruitmentAgencyCreate
{
    public class RecruitmentAgencyCreateHandler : IRequestHandler<RecruitmentAgencyCreateCommand, long>
    {
        private readonly IRecruitmentAgencyService _service;

        public RecruitmentAgencyCreateHandler(IRecruitmentAgencyService service)
        {
            _service = service;
        }

        public async Task<long> Handle(RecruitmentAgencyCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
