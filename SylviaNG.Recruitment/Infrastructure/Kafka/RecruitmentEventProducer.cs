using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace SylviaNG.Recruitment.Infrastructure.Kafka;

public interface IRecruitmentEventProducer
{
    Task PublishAsync(string topic, object eventPayload);
}

public class RecruitmentEventProducer : IRecruitmentEventProducer, IDisposable
{
    private readonly IProducer<string, string>? _producer;
    private readonly ILogger<RecruitmentEventProducer> _logger;
    private readonly bool _enabled;

    public RecruitmentEventProducer(IOptions<KafkaSettings> options, ILogger<RecruitmentEventProducer> logger)
    {
        _logger = logger;
        _enabled = !string.IsNullOrWhiteSpace(options.Value.BootstrapServers);

        if (_enabled)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = options.Value.BootstrapServers,
                Acks = Acks.Leader
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
    }

    public async Task PublishAsync(string topic, object eventPayload)
    {
        if (!_enabled || _producer == null)
        {
            _logger.LogDebug("Kafka not configured — skipping event publish to {Topic}", topic);
            return;
        }

        var json = JsonSerializer.Serialize(eventPayload);
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = json
        };

        try
        {
            var result = await _producer.ProduceAsync(topic, message);
            _logger.LogInformation("Published event to {Topic} [partition {Partition}, offset {Offset}]: {Payload}",
                topic, result.Partition.Value, result.Offset.Value, json);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to publish event to {Topic}: {Payload}", topic, json);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(5));
        _producer?.Dispose();
    }
}
