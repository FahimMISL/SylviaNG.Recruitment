using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MedicalLetters.Queries.MedicalLetterGetAll
{
    public class MedicalLetterGetAllHandler : IRequestHandler<MedicalLetterGetAllQuery, List<MedicalLetterResponse>>
    {
        private readonly IMedicalLetterService _medicalLetterService;

        public MedicalLetterGetAllHandler(IMedicalLetterService medicalLetterService)
        {
            _medicalLetterService = medicalLetterService;
        }

        public async Task<List<MedicalLetterResponse>> Handle(MedicalLetterGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _medicalLetterService.GetAllAsync(query.JobApplicationId);
        }
    }
}
