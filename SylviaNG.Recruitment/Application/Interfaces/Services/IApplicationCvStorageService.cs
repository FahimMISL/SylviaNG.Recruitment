namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// Abstraction over physical file storage for candidate CV/resume uploads submitted via
    /// the career portal / internal job board. Local-disk implementation lives in Infrastructure;
    /// no cloud storage SDK is referenced. Kept parallel to (and independent of) <see cref="IFileStorageService"/>
    /// so job-posting attachment storage is not touched by this feature.
    /// </summary>
    public interface IApplicationCvStorageService
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
