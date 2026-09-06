namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Renders a server-rendered office note body into a PDF. Same shape as
    /// IJoiningBookletPdfGeneratorService/IOfferLetterPdfGeneratorService - one generator interface
    /// per document type is this codebase's established convention.
    /// </summary>
    public interface IOfficeNotePdfGeneratorService
    {
        Task<byte[]> Generate(string documentTitle, string candidateName, string renderedBody, long sequenceValue);
    }
}
