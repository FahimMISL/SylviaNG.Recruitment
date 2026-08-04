using SylviaNG.Recruitment.Application.Common.Helpers;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class OfficeNoteMapper
    {
        public static OfficeNoteResponse ToResponse(this OfficeNote entity)
        {
            return new OfficeNoteResponse
            {
                OfficeNoteId = entity.OfficeNoteId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                DocumentTemplateId = entity.DocumentTemplateId,
                DocumentTemplateName = entity.DocumentTemplate?.Name ?? string.Empty,
                Remarks = entity.Remarks,
                EnclosuresSummary = entity.EnclosuresSummary,
                GeneratedPdfPath = FileUrlBuilder.BuildDownloadUrl(entity.GeneratedPdfPath) ?? string.Empty,
                GeneratedAt = entity.GeneratedAt,
            };
        }
    }
}
