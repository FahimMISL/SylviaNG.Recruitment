using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Local-disk implementation of <see cref="IFileStorageService"/>.
    /// Stores files under wwwroot/uploads/job-postings/{subFolder}/{guid}{ext} so they can be
    /// served directly by ASP.NET Core's static file middleware.
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FileStorageSettings _settings;

        public LocalFileStorageService(IWebHostEnvironment environment, IOptions<FileStorageSettings> options)
        {
            _environment = environment;
            _settings = options.Value;
        }

        public async Task<(string StoredFileName, string FilePath)> SaveAsync(Stream fileStream, string originalFileName, string subFolder)
        {
            var extension = Path.GetExtension(originalFileName);
            var storedFileName = $"{Guid.NewGuid():N}{extension}";

            var physicalDirectory = Path.Combine(_environment.ContentRootPath, _settings.RootPath, subFolder);
            Directory.CreateDirectory(physicalDirectory);

            var physicalFilePath = Path.Combine(physicalDirectory, storedFileName);
            await using (var destination = new FileStream(physicalFilePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(destination);
            }

            // Web-relative path (relative to wwwroot), used to build the static-file download URL.
            var webRelativeRoot = TrimWwwRootPrefix(_settings.RootPath);
            var relativeFilePath = string.Join('/', new[] { webRelativeRoot, subFolder, storedFileName }
                .Where(segment => !string.IsNullOrWhiteSpace(segment)));

            return (storedFileName, relativeFilePath);
        }

        public Task DeleteAsync(string relativeFilePath)
        {
            var physicalPath = Path.Combine(_environment.ContentRootPath, "wwwroot", relativeFilePath.TrimStart('/'));
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            return Task.CompletedTask;
        }

        private static string TrimWwwRootPrefix(string rootPath)
        {
            var normalized = rootPath.Replace('\\', '/').Trim('/');
            if (normalized.StartsWith("wwwroot/", StringComparison.OrdinalIgnoreCase))
                return normalized["wwwroot/".Length..];
            if (normalized.Equals("wwwroot", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            return normalized;
        }
    }
}
