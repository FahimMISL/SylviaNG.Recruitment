using MediatR;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Queries.NomineeGetAll
{
    public class NomineeGetAllHandler : IRequestHandler<NomineeGetAllQuery, List<NomineeResponse>>
    {
        private readonly INomineeService _service;

        public NomineeGetAllHandler(INomineeService service)
        {
            _service = service;
        }

        public async Task<List<NomineeResponse>> Handle(NomineeGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
