namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Resolves a caller-supplied relative file path against a fixed root and rejects anything
    /// that escapes it (../, absolute paths, drive-letter tricks) before it ever touches disk.
    /// Both local storage services take this path straight from FilesController's [AllowAnonymous]
    /// "key" query param, so without this check "../../../../appsettings.json" or similar reaches
    /// File.Open/File.Delete directly - Path.Combine alone does not stop ".." segments.
    /// </summary>
    internal static class SafeLocalPathResolver
    {
        public static string Resolve(string contentRootPath, string relativeFilePath)
        {
            var root = Path.GetFullPath(Path.Combine(contentRootPath, "wwwroot"));
            var candidate = Path.GetFullPath(Path.Combine(root, relativeFilePath.TrimStart('/', '\\')));

            if (!candidate.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && !candidate.Equals(root, StringComparison.OrdinalIgnoreCase))
            {
                throw new FileNotFoundException("Resolved path escapes the storage root.");
            }

            return candidate;
        }
    }
}
