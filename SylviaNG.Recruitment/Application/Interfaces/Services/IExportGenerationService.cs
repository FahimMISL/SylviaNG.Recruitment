namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public record ExportFileResult(byte[] Content, string ContentType, string FileName, int RowCount);

    /// <summary>
    /// EP-13 US-100/104: generates the actual export file from a fixed set of JobApplicationIds.
    /// Called only by ExportRequestWorker - never from an HTTP request thread, since building the
    /// workbook for a large candidate list is exactly the work US-104's queue exists to defer.
    /// </summary>
    public interface IExportGenerationService
    {
        Task<ExportFileResult> GenerateCandidateListExportAsync(
            List<long> jobApplicationIds,
            Domain.Enums.ExportFormatEnum format,
            CancellationToken cancellationToken = default);

        /// <summary>US-101: large-batch counterpart to JobApplicationService.BulkDownloadCvsAsync's
        /// synchronous path - same one-PDF-per-application ZIP, rendered on the worker thread.</summary>
        Task<ExportFileResult> GenerateBulkCvZipAsync(
            List<long> jobApplicationIds,
            CancellationToken cancellationToken = default);

        /// <summary>EP-14 US-109 AC5: one row per matched application, tracker columns
        /// (Vacancy/Candidate/Stage/Status/LastUpdated/DaysInStage/Stale/AssignedHR).</summary>
        Task<ExportFileResult> GenerateJobApplicationTrackerExportAsync(
            List<long> jobApplicationIds,
            Domain.Enums.ExportFormatEnum format,
            CancellationToken cancellationToken = default);
    }
}
