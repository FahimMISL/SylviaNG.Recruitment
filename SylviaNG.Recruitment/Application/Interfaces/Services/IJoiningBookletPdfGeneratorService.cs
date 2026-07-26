namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Renders a server-rendered joining booklet body into a PDF. Same shape as
    /// IAppointmentLetterPdfGeneratorService/IOfferLetterPdfGeneratorService - one generator
    /// interface per document type is this codebase's established convention.
    /// </summary>
    public interface IJoiningBookletPdfGeneratorService
    {
        byte[] Generate(string documentTitle, string candidateName, string renderedBody);
    }
}
