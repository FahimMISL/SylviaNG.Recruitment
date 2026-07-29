using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class TargetLetterMapper
    {
        public static TargetLetterResponse ToResponse(this TargetLetter entity)
        {
            return new TargetLetterResponse
            {
                TargetLetterId = entity.TargetLetterId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                OfferLetterId = entity.OfferLetterId,
                DocumentTemplateId = entity.DocumentTemplateId,
                DocumentTemplateName = entity.DocumentTemplate?.Name ?? string.Empty,
                Kpis = entity.Kpis,
                Objectives = entity.Objectives,
                FinalBody = entity.FinalBody,
                GeneratedPdfPath = entity.GeneratedPdfPath,
                GeneratedAt = entity.GeneratedAt,
            };
        }
    }
}
