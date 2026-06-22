using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Queries.MedicalTestGetById
{
    public class MedicalTestGetByIdHandler : IRequestHandler<MedicalTestGetByIdQuery, MedicalTestResponse>
    {
        private readonly IMedicalTestService _service;

        public MedicalTestGetByIdHandler(IMedicalTestService service)
        {
            _service = service;
        }

        public async Task<MedicalTestResponse> Handle(MedicalTestGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.MedicalTestId);
        }
    }
}
