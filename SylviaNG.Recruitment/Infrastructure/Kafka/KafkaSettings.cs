namespace SylviaNG.Recruitment.Infrastructure.Kafka
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupId { get; set; } = "sylviang-recruitment-employee-sync";
    }
}
