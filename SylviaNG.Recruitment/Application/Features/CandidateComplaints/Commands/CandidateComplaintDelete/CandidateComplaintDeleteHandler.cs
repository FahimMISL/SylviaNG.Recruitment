using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintDelete
{
    public class CandidateComplaintDeleteHandler : IRequestHandler<CandidateComplaintDeleteCommand, Unit>
    {
        private readonly ICandidateComplaintService _service;

        public CandidateComplaintDeleteHandler(ICandidateComplaintService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(CandidateComplaintDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.CandidateComplaintId);
            return Unit.Value;
        }
    }
}
