using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MedicalLetters.Commands.MedicalLetterGenerate
{
    public class MedicalLetterGenerateHandler : IRequestHandler<MedicalLetterGenerateCommand, MedicalLetterResponse>
    {
        private readonly IMedicalLetterService _medicalLetterService;

        public MedicalLetterGenerateHandler(IMedicalLetterService medicalLetterService)
        {
            _medicalLetterService = medicalLetterService;
        }

        public async Task<MedicalLetterResponse> Handle(MedicalLetterGenerateCommand command, CancellationToken cancellationToken)
        {
            return await _medicalLetterService.GenerateAsync(command.Request);
        }
    }
}
