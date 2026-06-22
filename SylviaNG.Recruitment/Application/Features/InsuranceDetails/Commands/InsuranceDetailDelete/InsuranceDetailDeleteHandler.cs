using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailDelete
{
    public class InsuranceDetailDeleteHandler : IRequestHandler<InsuranceDetailDeleteCommand, Unit>
    {
        private readonly IInsuranceDetailService _service;

        public InsuranceDetailDeleteHandler(IInsuranceDetailService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(InsuranceDetailDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.InsuranceDetailId);
            return Unit.Value;
        }
    }
}
