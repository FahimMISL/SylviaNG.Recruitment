namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Renders an already HR-reviewed appointment letter body into a PDF. Same shape as
    /// IOfferLetterPdfGeneratorService - one generator interface per document type is this
    /// codebase's established convention (see also ICvPdfGeneratorService, IAdmitCardPdfGeneratorService).
    /// </summary>
    public interface IAppointmentLetterPdfGeneratorService
    {
        byte[] Generate(string documentTitle, string candidateName, string finalBody);
    }
}
