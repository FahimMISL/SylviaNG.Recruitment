using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MedicalLetters.Queries.MedicalLetterGetById
{
    public class MedicalLetterGetByIdHandler : IRequestHandler<MedicalLetterGetByIdQuery, MedicalLetterResponse>
    {
        private readonly IMedicalLetterService _medicalLetterService;

        public MedicalLetterGetByIdHandler(IMedicalLetterService medicalLetterService)
        {
            _medicalLetterService = medicalLetterService;
        }

        public async Task<MedicalLetterResponse> Handle(MedicalLetterGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _medicalLetterService.GetByIdAsync(query.MedicalLetterId);
        }
    }
}
