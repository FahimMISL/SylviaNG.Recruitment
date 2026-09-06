using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SylviaNG.Recruitment.Application.Common.Settings;

namespace SylviaNG.Recruitment.Infrastructure.BackgroundServices
{
    /// <summary>
    /// One-shot startup bucket bootstrap for the MinIO file-storage provider. MinIO does not
    /// auto-create buckets on first PutObject (unlike some S3-compatible providers), so this
    /// runs once at app start, idempotently, instead of requiring a manual `mc mb` step.
    /// </summary>
    public class MinioBucketInitializer : IHostedService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _settings;

        public MinioBucketInitializer(IMinioClient minioClient, IOptions<MinioSettings> options)
        {
            _minioClient = minioClient;
            _settings = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var existsArgs = new BucketExistsArgs().WithBucket(_settings.BucketName);
            if (!await _minioClient.BucketExistsAsync(existsArgs, cancellationToken))
            {
                var makeArgs = new MakeBucketArgs().WithBucket(_settings.BucketName);
                await _minioClient.MakeBucketAsync(makeArgs, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
