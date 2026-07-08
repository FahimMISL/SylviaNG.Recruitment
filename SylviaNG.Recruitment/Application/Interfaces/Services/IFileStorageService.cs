namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Abstraction over physical file storage for uploaded attachments.
    /// Local-disk implementation lives in Infrastructure; no cloud storage SDK is referenced.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Persists the given stream under the configured storage root, inside <paramref name="subFolder"/>.
        /// Returns the generated stored file name and the web-relative file path (relative to wwwroot),
        /// which can be used to build a static-file download URL and for later deletion.
        /// </summary>
        Task<(string StoredFileName, string FilePath)> SaveAsync(Stream fileStream, string originalFileName, string subFolder);

        /// <summary>
        /// Deletes a previously saved file given its web-relative file path (relative to wwwroot).
        /// </summary>
        Task DeleteAsync(string relativeFilePath);
    }
}
