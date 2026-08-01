using MediatR;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.MedicalLetters.Commands.MedicalLetterGenerate
{
    public class MedicalLetterGenerateCommand : IRequest<MedicalLetterResponse>
    {
        public MedicalLetterGenerateRequest Request { get; set; }

        public MedicalLetterGenerateCommand(MedicalLetterGenerateRequest request)
        {
            Request = request;
        }
    }
}
