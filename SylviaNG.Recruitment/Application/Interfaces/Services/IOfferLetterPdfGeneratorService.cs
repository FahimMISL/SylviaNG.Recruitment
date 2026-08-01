namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Renders an already placeholder-substituted offer letter body into a PDF. Takes plain
    /// rendered text (not the DocumentTemplate entity) so it stays decoupled from where the body
    /// came from - same separation as QuestPdfAdmitCardGenerator taking domain entities directly,
    /// except here the substitution already happened via IPlaceholderSubstitutionService upstream.
    /// </summary>
    public interface IOfferLetterPdfGeneratorService
    {
        Task<byte[]> Generate(string documentTitle, string candidateName, string renderedBody, long sequenceValue);
    }
}
