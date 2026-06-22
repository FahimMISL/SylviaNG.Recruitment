using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Queries.MedicalTestGetAll
{
    public class MedicalTestGetAllHandler : IRequestHandler<MedicalTestGetAllQuery, List<MedicalTestResponse>>
    {
        private readonly IMedicalTestService _service;

        public MedicalTestGetAllHandler(IMedicalTestService service)
        {
            _service = service;
        }

        public async Task<List<MedicalTestResponse>> Handle(MedicalTestGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
