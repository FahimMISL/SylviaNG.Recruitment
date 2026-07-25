using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class OfferLetterMapper
    {
        public static OfferLetterResponse ToResponse(this OfferLetter entity)
        {
            return new OfferLetterResponse
            {
                OfferLetterId = entity.OfferLetterId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                DocumentTemplateId = entity.DocumentTemplateId,
                DocumentTemplateName = entity.DocumentTemplate?.Name ?? string.Empty,
                Designation = entity.Designation,
                OfferedSalary = entity.OfferedSalary,
                JoiningDate = entity.JoiningDate,
                ReportingManager = entity.ReportingManager,
                OfferValidityDate = entity.OfferValidityDate,
                GeneratedPdfPath = entity.GeneratedPdfPath,
                Status = entity.Status,
                GeneratedAt = entity.GeneratedAt,
            };
        }
    }
}
