using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintCreate
{
    public class CandidateComplaintCreateHandler : IRequestHandler<CandidateComplaintCreateCommand, long>
    {
        private readonly ICandidateComplaintService _service;

        public CandidateComplaintCreateHandler(ICandidateComplaintService service)
        {
            _service = service;
        }

        public async Task<long> Handle(CandidateComplaintCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
