using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailUpdate
{
    public class InsuranceDetailUpdateHandler : IRequestHandler<InsuranceDetailUpdateCommand, Unit>
    {
        private readonly IInsuranceDetailService _service;

        public InsuranceDetailUpdateHandler(IInsuranceDetailService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InsuranceDetailUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.InsuranceDetailId, command.Request);
            return Unit.Value;
        }
    }
}
