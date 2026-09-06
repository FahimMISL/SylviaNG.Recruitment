namespace SylviaNG.Recruitment.Application.Common.Settings
{
    /// <summary>
    /// Options bound from the "Minio" configuration section. Only read when
    /// FileStorage:Provider is "Minio" - see DependencyInjection's provider switch.
    /// </summary>
    public class MinioSettings
    {
        public const string SectionName = "Minio";

        public string Endpoint { get; set; } = "localhost:9100";
        public string AccessKey { get; set; } = "minioadmin";
        public string SecretKey { get; set; } = "minioadmin123";
        public bool UseSsl { get; set; } = false;
        public string BucketName { get; set; } = "sylviang-recruitment";
    }
}
