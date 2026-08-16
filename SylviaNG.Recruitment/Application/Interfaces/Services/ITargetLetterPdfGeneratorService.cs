namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Renders an already HR-reviewed target letter body into a PDF. Same shape as
    /// IAppointmentLetterPdfGeneratorService - one generator interface per document type.
    /// </summary>
    public interface ITargetLetterPdfGeneratorService
    {
        Task<byte[]> Generate(string documentTitle, string candidateName, string finalBody, long sequenceValue);
    }
}
