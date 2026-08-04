using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    // EP-10 US-084: joining booklets for a batch of Accepted-offer candidates. AC1's "Final
    // Selection Pool" is US-094/EP-12 which doesn't exist yet (scheduled after EP-10) - this is
    // the lightweight batch stub: HR picks Accepted-offer candidates directly and enters a
    // BatchLabel/JoiningDate ad hoc, same "fitment-data hook" pattern OfferLetter itself already
    // uses for Designation/ReportingManager. GenerateForOfferLetterAsync is the one shared core
    // used by both the single-candidate and bulk paths so PDF-generation logic is never duplicated.
    public class JoiningBookletService : IJoiningBookletService
    {
        private readonly IJoiningBookletRepository _joiningBookletRepository;
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly IDocumentTemplateRepository _documentTemplateRepository;
        private readonly IPlaceholderSubstitutionService _placeholderSubstitutionService;
        private readonly IJoiningBookletPdfGeneratorService _joiningBookletPdfGeneratorService;
        private readonly IFileStorageService _fileStorageService;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly PortalSettings _portalSettings;
        private readonly IUnitOfWork _unitOfWork;

        private const string PdfStorageSubFolder = "documents/joining-booklets";

        public JoiningBookletService(
            IJoiningBookletRepository joiningBookletRepository,
            IOfferLetterRepository offerLetterRepository,
            IDocumentTemplateRepository documentTemplateRepository,
            IPlaceholderSubstitutionService placeholderSubstitutionService,
            IJoiningBookletPdfGeneratorService joiningBookletPdfGeneratorService,
            IFileStorageService fileStorageService,
            INotificationDispatchService notificationDispatchService,
            IApplicationSettingService applicationSettingService,
            IOptions<PortalSettings> portalSettings,
            IUnitOfWork unitOfWork)
        {
            _joiningBookletRepository = joiningBookletRepository;
            _offerLetterRepository = offerLetterRepository;
            _documentTemplateRepository = documentTemplateRepository;
            _placeholderSubstitutionService = placeholderSubstitutionService;
            _joiningBookletPdfGeneratorService = joiningBookletPdfGeneratorService;
            _fileStorageService = fileStorageService;
            _notificationDispatchService = notificationDispatchService;
            _applicationSettingService = applicationSettingService;
            _portalSettings = portalSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<JoiningBookletEligibleCandidateResponse>> GetEligibleCandidatesAsync()
        {
            var offerLetters = await _offerLetterRepository.GetAcceptedOrderedAsync();
            return offerLetters.Select(o => o.ToEligibleCandidateResponse()).ToList();
        }

        public async Task<JoiningBookletResponse> GenerateAsync(JoiningBookletGenerateRequest request)
        {
            var offerLetter = await _offerLetterRepository.GetByIdWithDetailsAsync(request.OfferLetterId)
                ?? throw new NotFoundException("OfferLetter", request.OfferLetterId);
            EnsureAccepted(offerLetter);

            var template = await GetValidatedTemplateAsync(request.DocumentTemplateId);

            var entity = await GenerateForOfferLetterAsync(offerLetter, template, request.BatchLabel, request.JoiningDate);
            return entity.ToResponse();
        }

        public async Task<JoiningBookletBulkGenerateResponse> BulkGenerateAsync(JoiningBookletBulkGenerateRequest request)
        {
            var offerLetterIds = request.OfferLetterIds.Distinct().ToList();
            if (offerLetterIds.Count == 0)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.OfferLetterIds), "Select at least one candidate.")
                });
            }

            var template = await GetValidatedTemplateAsync(request.DocumentTemplateId);
            var offerLetters = await _offerLetterRepository.GetByIdsWithDetailsAsync(offerLetterIds);
            var offerLettersById = offerLetters.ToDictionary(o => o.OfferLetterId);

            var results = new List<JoiningBookletBulkItemResult>();
            foreach (var offerLetterId in offerLetterIds)
            {
                try
                {
                    if (!offerLettersById.TryGetValue(offerLetterId, out var offerLetter))
                        throw new NotFoundException("OfferLetter", offerLetterId);
                    EnsureAccepted(offerLetter);

                    var entity = await GenerateForOfferLetterAsync(offerLetter, template, request.BatchLabel, request.JoiningDate);
                    results.Add(new JoiningBookletBulkItemResult
                    {
                        OfferLetterId = offerLetterId,
                        Success = true,
                        JoiningBookletId = entity.JoiningBookletId,
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new JoiningBookletBulkItemResult
                    {
                        OfferLetterId = offerLetterId,
                        Success = false,
                        ErrorMessage = ex.Message,
                    });
                }
            }

            return new JoiningBookletBulkGenerateResponse
            {
                TotalRequested = offerLetterIds.Count,
                SuccessCount = results.Count(r => r.Success),
                FailureCount = results.Count(r => !r.Success),
                Results = results,
            };
        }

        public async Task<JoiningBookletFileResponse> BulkDownloadAsync(JoiningBookletBulkDownloadRequest request)
        {
            var joiningBookletIds = request.JoiningBookletIds.Distinct().ToList();
            if (joiningBookletIds.Count == 0)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.JoiningBookletIds), "Select at least one generated booklet.")
                });
            }

            var booklets = await _joiningBookletRepository.GetByIdsWithDetailsAsync(joiningBookletIds);

            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var usedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var booklet in booklets)
                {
                    var fileName = ToPdfFileName(booklet.JobApplication.CandidateName, booklet.JoiningBookletId);
                    while (!usedFileNames.Add(fileName))
                        fileName = ToPdfFileName(booklet.JobApplication.CandidateName + "_", booklet.JoiningBookletId);

                    var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                    await using var entryStream = entry.Open();
                    var pdfBytes = await _joiningBookletPdfGeneratorService.Generate(
                        booklet.DocumentTemplate.Name, booklet.JobApplication.CandidateName, booklet.RenderedBody, booklet.JoiningBookletId);
                    await entryStream.WriteAsync(pdfBytes);
                }
            }

            return new JoiningBookletFileResponse
            {
                Content = zipStream.ToArray(),
                ContentType = "application/zip",
                FileName = $"Joining-Booklets-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip",
            };
        }

        public async Task<List<JoiningBookletResponse>> GetAllAsync(long? jobApplicationId)
        {
            var entities = await _joiningBookletRepository.GetAllOrderedAsync(jobApplicationId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<JoiningBookletResponse> GetByIdAsync(long joiningBookletId)
        {
            var entity = await _joiningBookletRepository.GetByIdWithDetailsAsync(joiningBookletId)
                ?? throw new NotFoundException("JoiningBooklet", joiningBookletId);

            return entity.ToResponse();
        }

        private async Task<JoiningBooklet> GenerateForOfferLetterAsync(
            OfferLetter offerLetter, DocumentTemplate template, string batchLabel, DateTime joiningDate)
        {
            var placeholderValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = offerLetter.JobApplication.CandidateName,
                ["Designation"] = offerLetter.Designation,
                ["BatchLabel"] = batchLabel,
                ["JoiningDate"] = joiningDate.ToString("dd MMM yyyy"),
                ["PortalLink"] = $"{_portalSettings.FrontendBaseUrl}/candidate-profile/offer-letters",
            };
            var renderedBody = _placeholderSubstitutionService.Render(template.Body, placeholderValues);

            var pdfBytes = await _joiningBookletPdfGeneratorService.Generate(
                template.Name, offerLetter.JobApplication.CandidateName, renderedBody, offerLetter.OfferLetterId);

            using var pdfStream = new MemoryStream(pdfBytes);
            var (_, filePath) = await _fileStorageService.SaveAsync(pdfStream, "joining-booklet.pdf", PdfStorageSubFolder);

            var entity = new JoiningBooklet
            {
                JobApplicationId = offerLetter.JobApplicationId,
                OfferLetterId = offerLetter.OfferLetterId,
                DocumentTemplateId = template.DocumentTemplateId,
                BatchLabel = batchLabel,
                JoiningDate = joiningDate,
                RenderedBody = renderedBody,
                GeneratedPdfPath = filePath,
                GeneratedAt = DateTime.UtcNow,
            };

            await _joiningBookletRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            entity.JobApplication = offerLetter.JobApplication;
            entity.OfferLetter = offerLetter;
            entity.DocumentTemplate = template;

            // Never throws - a missing EventTemplateMapping just logs a Skipped NotificationLog row.
            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.JoiningBookletAvailable,
                placeholderValues,
                new NotificationDispatchTargets(offerLetter.JobApplication.CandidateEmail, await _applicationSettingService.GetHrNotificationEmailAsync(), offerLetter.JobApplicationId),
                persistImmediately: true);

            return entity;
        }

        private async Task<DocumentTemplate> GetValidatedTemplateAsync(long documentTemplateId)
        {
            var template = await _documentTemplateRepository.GetByIdAsync(documentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", documentTemplateId);

            if (template.DocumentType != DocumentTypeEnum.JoiningBooklet)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(documentTemplateId),
                        "The selected template is not a JoiningBooklet-type DocumentTemplate.")
                });

            if (!template.IsActive)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(documentTemplateId), "The selected template is inactive.")
                });

            return template;
        }

        private static void EnsureAccepted(OfferLetter offerLetter)
        {
            if (offerLetter.Status != OfferLetterStatusEnum.Accepted)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(offerLetter.OfferLetterId),
                        "A joining booklet can only be generated for an Accepted offer letter.")
                });
        }

        private static string ToPdfFileName(string candidateName, long joiningBookletId)
        {
            var safeName = Regex.Replace(candidateName, @"[^a-zA-Z0-9\-]+", "_").Trim('_');
            if (string.IsNullOrEmpty(safeName))
                safeName = "candidate";

            return $"{safeName}_{joiningBookletId}_JoiningBooklet.pdf";
        }
    }
}
