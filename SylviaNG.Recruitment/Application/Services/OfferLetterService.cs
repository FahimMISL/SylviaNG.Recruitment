using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class OfferLetterService : IOfferLetterService
    {
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly IDocumentTemplateRepository _documentTemplateRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IPlaceholderSubstitutionService _placeholderSubstitutionService;
        private readonly IOfferLetterPdfGeneratorService _offerLetterPdfGeneratorService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        private const string PdfStorageSubFolder = "documents/offer-letters";

        public OfferLetterService(
            IOfferLetterRepository offerLetterRepository,
            IDocumentTemplateRepository documentTemplateRepository,
            IJobApplicationRepository jobApplicationRepository,
            IPlaceholderSubstitutionService placeholderSubstitutionService,
            IOfferLetterPdfGeneratorService offerLetterPdfGeneratorService,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _offerLetterRepository = offerLetterRepository;
            _documentTemplateRepository = documentTemplateRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _placeholderSubstitutionService = placeholderSubstitutionService;
            _offerLetterPdfGeneratorService = offerLetterPdfGeneratorService;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<OfferLetterResponse> GenerateAsync(OfferLetterGenerateRequest request)
        {
            var template = await _documentTemplateRepository.GetByIdAsync(request.DocumentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", request.DocumentTemplateId);

            if (template.DocumentType != DocumentTypeEnum.OfferLetter)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId),
                        "The selected template is not an OfferLetter-type DocumentTemplate.")
                });

            if (!template.IsActive)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId),
                        "The selected template is inactive.")
                });

            var jobApplication = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == request.JobApplicationId,
                a => a.CandidateProfile!,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", request.JobApplicationId);

            var placeholderValues = BuildPlaceholderValues(jobApplication, request);
            var renderedBody = _placeholderSubstitutionService.Render(template.Body, placeholderValues);

            var pdfBytes = _offerLetterPdfGeneratorService.Generate(template.Name, jobApplication.CandidateName, renderedBody);

            using var pdfStream = new MemoryStream(pdfBytes);
            var (_, filePath) = await _fileStorageService.SaveAsync(pdfStream, "offer-letter.pdf", PdfStorageSubFolder);

            var entity = new OfferLetter
            {
                JobApplicationId = request.JobApplicationId,
                DocumentTemplateId = request.DocumentTemplateId,
                Designation = request.Designation,
                OfferedSalary = request.OfferedSalary,
                JoiningDate = request.JoiningDate,
                ReportingManager = request.ReportingManager,
                OfferValidityDate = request.OfferValidityDate,
                GeneratedPdfPath = filePath,
                Status = OfferLetterStatusEnum.Generated,
                GeneratedAt = DateTime.UtcNow,
            };

            await _offerLetterRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            entity.JobApplication = jobApplication;
            entity.DocumentTemplate = template;
            return entity.ToResponse();
        }

        public async Task<List<OfferLetterResponse>> GetAllAsync(long? jobApplicationId)
        {
            var entities = await _offerLetterRepository.GetAllOrderedAsync(jobApplicationId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<OfferLetterResponse> GetByIdAsync(long offerLetterId)
        {
            var entity = await _offerLetterRepository.GetByIdWithDetailsAsync(offerLetterId)
                ?? throw new NotFoundException("OfferLetter", offerLetterId);

            return entity.ToResponse();
        }

        private static Dictionary<string, string> BuildPlaceholderValues(JobApplication jobApplication, OfferLetterGenerateRequest request)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = jobApplication.CandidateName,
                ["CandidateEmail"] = jobApplication.CandidateEmail ?? string.Empty,
                ["JobTitle"] = jobApplication.JobPosting?.Title ?? string.Empty,
                ["Designation"] = request.Designation,
                ["OfferedSalary"] = request.OfferedSalary.ToString("N2"),
                ["JoiningDate"] = request.JoiningDate.ToString("dd MMM yyyy"),
                ["ReportingManager"] = request.ReportingManager ?? string.Empty,
                ["OfferValidityDate"] = request.OfferValidityDate?.ToString("dd MMM yyyy") ?? string.Empty,
            };
        }
    }
}
