using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintUpdate
{
    public class CandidateComplaintUpdateHandler : IRequestHandler<CandidateComplaintUpdateCommand, Unit>
    {
        private readonly ICandidateComplaintService _service;

        public CandidateComplaintUpdateHandler(ICandidateComplaintService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateComplaintUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.CandidateComplaintId, command.Request);
            return Unit.Value;
        }
    }
}
