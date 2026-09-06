using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// MinIO (S3-compatible) implementation of <see cref="IApplicationCvStorageService"/>, backing
    /// career-portal / internal-job-board CV uploads. Object key mirrors the local-disk relative
    /// path shape ("applications/{subFolder}/{guid}{ext}"). Shares one bucket with
    /// <see cref="MinioFileStorageService"/>, separated by key prefix.
    /// </summary>
    public class MinioApplicationCvStorageService : IApplicationCvStorageService
    {
        private const string RootPrefix = "applications";
        private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _settings;

        public MinioApplicationCvStorageService(IMinioClient minioClient, IOptions<MinioSettings> options)
        {
            _minioClient = minioClient;
            _settings = options.Value;
        }

        public async Task<(string StoredFileName, string FilePath)> SaveAsync(Stream fileStream, string originalFileName, string subFolder)
        {
            var extension = Path.GetExtension(originalFileName);
            var storedFileName = $"{Guid.NewGuid():N}{extension}";
            var objectKey = string.Join('/', new[] { RootPrefix, subFolder, storedFileName }
                .Where(segment => !string.IsNullOrWhiteSpace(segment)));

            var contentType = GetContentType(extension);

            var putArgs = new PutObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(objectKey)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putArgs);

            return (storedFileName, objectKey);
        }

        public async Task DeleteAsync(string relativeFilePath)
        {
            var removeArgs = new RemoveObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(relativeFilePath.TrimStart('/'));
            await _minioClient.RemoveObjectAsync(removeArgs);
        }

        public async Task<Stream> OpenReadAsync(string relativeFilePath)
        {
            var memoryStream = new MemoryStream();
            try
            {
                var getArgs = new GetObjectArgs()
                    .WithBucket(_settings.BucketName)
                    .WithObject(relativeFilePath.TrimStart('/'))
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream));

                await _minioClient.GetObjectAsync(getArgs);
            }
            catch (Minio.Exceptions.ObjectNotFoundException ex)
            {
                throw new FileNotFoundException($"Object not found: {relativeFilePath}", ex);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        private static string GetContentType(string extension) =>
            ContentTypeProvider.TryGetContentType($"file{extension}", out var contentType)
                ? contentType
                : "application/octet-stream";
    }
}
