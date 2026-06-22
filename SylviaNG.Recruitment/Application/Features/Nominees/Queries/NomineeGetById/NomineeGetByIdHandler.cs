using MediatR;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Queries.NomineeGetById
{
    public class NomineeGetByIdHandler : IRequestHandler<NomineeGetByIdQuery, NomineeResponse>
    {
        private readonly INomineeService _service;

        public NomineeGetByIdHandler(INomineeService service)
        {
            _service = service;
        }

        public async Task<NomineeResponse> Handle(NomineeGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.NomineeId);
        }
    }
}
