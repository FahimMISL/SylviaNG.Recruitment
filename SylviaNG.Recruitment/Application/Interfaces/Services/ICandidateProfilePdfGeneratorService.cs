using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>US-103: standardized, branded profile summary PDF for interview panels - distinct
    /// from ICvPdfGeneratorService's CV layout (adds a screening-score line, omits the "Curriculum
    /// Vitae" framing).</summary>
    public interface ICandidateProfilePdfGeneratorService
    {
        Task<byte[]> Generate(CandidateProfile profile, int? screeningScore);
    }
}
