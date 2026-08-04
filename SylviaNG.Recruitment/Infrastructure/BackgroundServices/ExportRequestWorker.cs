using System.Text.Json;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.BackgroundServices
{
    /// <summary>
    /// EP-13 US-104: polls for Pending ExportRequest rows and renders them off the HTTP request
    /// thread, then dispatches an EP-09 notification and sweeps expired rows on the same tick.
    /// Polling (not a message queue) because Kafka is disabled in this environment (no broker
    /// reachable) - same _serviceProvider.CreateScope() shape as EmployeeEventConsumer, the only
    /// other BackgroundService in this codebase, for the scoped DB/service access a singleton
    /// hosted service needs.
    /// </summary>
    public class ExportRequestWorker : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
        private const int BatchSize = 5;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExportRequestWorker> _logger;

        public ExportRequestWorker(IServiceProvider serviceProvider, ILogger<ExportRequestWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExportRequestWorker starting...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var exportRequestRepository = scope.ServiceProvider.GetRequiredService<IExportRequestRepository>();
                    var exportGenerationService = scope.ServiceProvider.GetRequiredService<IExportGenerationService>();
                    var notificationDispatchService = scope.ServiceProvider.GetRequiredService<INotificationDispatchService>();
                    var fileStorageService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    await ProcessPendingAsync(exportRequestRepository, exportGenerationService, notificationDispatchService, fileStorageService, unitOfWork, stoppingToken);
                    await SweepExpiredAsync(exportRequestRepository, fileStorageService, unitOfWork);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ExportRequestWorker tick");
                }

                try
                {
                    await Task.Delay(PollInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task ProcessPendingAsync(
            IExportRequestRepository exportRequestRepository,
            IExportGenerationService exportGenerationService,
            INotificationDispatchService notificationDispatchService,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork,
            CancellationToken stoppingToken)
        {
            var pending = await exportRequestRepository.GetPendingOldestFirstAsync(BatchSize);

            foreach (var entity in pending)
            {
                entity.Status = ExportRequestStatusEnum.Processing;
                exportRequestRepository.Update(entity);
                await unitOfWork.SaveChangesAsync();

                var targets = new NotificationDispatchTargets(CandidateEmail: null, AdminHrEmail: entity.RequestedByEmail);

                try
                {
                    var jobApplicationIds = JsonSerializer.Deserialize<List<long>>(entity.JobApplicationIdsJson) ?? new List<long>();
                    var file = entity.ExportType switch
                    {
                        ExportTypeEnum.BulkCvZip => await exportGenerationService.GenerateBulkCvZipAsync(jobApplicationIds, stoppingToken),
                        ExportTypeEnum.JobApplicationTrackerExport => await exportGenerationService.GenerateJobApplicationTrackerExportAsync(jobApplicationIds, entity.Format, stoppingToken),
                        _ => await exportGenerationService.GenerateCandidateListExportAsync(jobApplicationIds, entity.Format, stoppingToken)
                    };

                    using var contentStream = new MemoryStream(file.Content);
                    var (_, objectKey) = await fileStorageService.SaveAsync(contentStream, file.FileName, "export-requests");
                    entity.ContentObjectKey = objectKey;
                    entity.ContentType = file.ContentType;
                    entity.FileName = file.FileName;
                    entity.RowCount = file.RowCount;
                    entity.Status = ExportRequestStatusEnum.Completed;
                    entity.CompletedAt = DateTime.UtcNow;
                    exportRequestRepository.Update(entity);
                    await unitOfWork.SaveChangesAsync();

                    await notificationDispatchService.DispatchAsync(
                        RecruitmentEventEnum.ExportRequestReady,
                        new Dictionary<string, string> { ["FileName"] = file.FileName, ["RowCount"] = file.RowCount.ToString() },
                        targets,
                        cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate ExportRequest {ExportRequestId}", entity.ExportRequestId);

                    entity.Status = ExportRequestStatusEnum.Failed;
                    entity.FailureReason = ex.Message;
                    exportRequestRepository.Update(entity);
                    await unitOfWork.SaveChangesAsync();

                    await notificationDispatchService.DispatchAsync(
                        RecruitmentEventEnum.ExportRequestFailed,
                        new Dictionary<string, string> { ["FailureReason"] = ex.Message },
                        targets,
                        cancellationToken: stoppingToken);
                }
            }
        }

        private async Task SweepExpiredAsync(IExportRequestRepository exportRequestRepository, IFileStorageService fileStorageService, IUnitOfWork unitOfWork)
        {
            var expired = await exportRequestRepository.GetExpiredAsync(DateTime.UtcNow);
            if (expired.Count == 0)
                return;

            foreach (var entity in expired.Where(e => e.ContentObjectKey != null))
            {
                try
                {
                    await fileStorageService.DeleteAsync(entity.ContentObjectKey!);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete expired export object {Key}", entity.ContentObjectKey);
                }
            }

            exportRequestRepository.DeleteRange(expired);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
