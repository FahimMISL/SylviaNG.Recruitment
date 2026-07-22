using System.IO.Compression;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankCvBulkDownload
{
    /// <summary>One PDF per candidate, zipped, so each CV stays a separate usable file.</summary>
    public class CvBankCvBulkDownloadHandler : IRequestHandler<CvBankCvBulkDownloadQuery, CvBankCvFileResponse>
    {
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly ICvPdfGeneratorService _cvPdfGeneratorService;

        public CvBankCvBulkDownloadHandler(ICandidateProfileRepository candidateProfileRepository, ICvPdfGeneratorService cvPdfGeneratorService)
        {
            _candidateProfileRepository = candidateProfileRepository;
            _cvPdfGeneratorService = cvPdfGeneratorService;
        }

        public async Task<CvBankCvFileResponse> Handle(CvBankCvBulkDownloadQuery query, CancellationToken cancellationToken)
        {
            var candidateProfileIds = query.Request.CandidateProfileIds.Distinct().ToList();
            if (candidateProfileIds.Count == 0)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(CvBankCvBulkRequest.CandidateProfileIds), "Select at least one candidate.")
                });
            }

            var profiles = await _candidateProfileRepository.GetByIdsWithDetailsAsync(candidateProfileIds);

            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var usedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var profile in profiles)
                {
                    var fileName = CvFileNaming.ToPdfFileName(profile.FullName, profile.CandidateProfileId);
                    while (!usedFileNames.Add(fileName))
                        fileName = CvFileNaming.ToPdfFileName(profile.FullName + "_", profile.CandidateProfileId);

                    var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                    await using var entryStream = entry.Open();
                    var pdfBytes = _cvPdfGeneratorService.Generate(profile);
                    await entryStream.WriteAsync(pdfBytes, cancellationToken);
                }
            }

            return new CvBankCvFileResponse
            {
                Content = zipStream.ToArray(),
                ContentType = "application/zip",
                FileName = $"CV-Bank-Export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip"
            };
        }
    }
}
