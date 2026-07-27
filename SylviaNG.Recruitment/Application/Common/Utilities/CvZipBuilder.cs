using System.IO.Compression;
using SylviaNG.Recruitment.Application.Features.CvBank;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Common.Utilities
{
    /// <summary>
    /// US-101: shared by JobApplicationService.BulkDownloadCvsAsync (small, synchronous batches)
    /// and ExportGenerationService.GenerateBulkCvZipAsync (large batches via the EP-13 F1 async
    /// queue) - same in-memory MemoryStream/ZipArchive/dedup-suffix pattern CvBankCvBulkDownloadHandler
    /// established, just keyed by JobApplicationId/CandidateName instead of CandidateProfileId.
    /// </summary>
    internal static class CvZipBuilder
    {
        public static async Task<byte[]> BuildAsync(
            IEnumerable<(long JobApplicationId, string CandidateName, CandidateProfile Profile)> items,
            ICvPdfGeneratorService cvPdfGeneratorService,
            CancellationToken cancellationToken = default)
        {
            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var usedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var (jobApplicationId, candidateName, profile) in items)
                {
                    var fileName = CvFileNaming.ToApplicationCvFileName(candidateName, jobApplicationId);
                    while (!usedFileNames.Add(fileName))
                        fileName = CvFileNaming.ToApplicationCvFileName(candidateName + "_", jobApplicationId);

                    var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                    await using var entryStream = entry.Open();
                    var pdfBytes = cvPdfGeneratorService.Generate(profile);
                    await entryStream.WriteAsync(pdfBytes, cancellationToken);
                }
            }

            return zipStream.ToArray();
        }
    }
}
