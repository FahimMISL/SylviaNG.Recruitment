using SylviaNG.Recruitment.Application.Common.Helpers;
using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class MedicalLetterMapper
    {
        public static MedicalLetterResponse ToResponse(this MedicalLetter entity)
        {
            return new MedicalLetterResponse
            {
                MedicalLetterId = entity.MedicalLetterId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                OfferLetterId = entity.OfferLetterId,
                DocumentTemplateId = entity.DocumentTemplateId,
                DocumentTemplateName = entity.DocumentTemplate?.Name ?? string.Empty,
                MedicalTestCenter = entity.MedicalTestCenter,
                RequiredTests = entity.RequiredTests,
                FinalBody = entity.FinalBody,
                GeneratedPdfPath = FileUrlBuilder.BuildDownloadUrl(entity.GeneratedPdfPath) ?? string.Empty,
                GeneratedAt = entity.GeneratedAt,
            };
        }
    }
}
