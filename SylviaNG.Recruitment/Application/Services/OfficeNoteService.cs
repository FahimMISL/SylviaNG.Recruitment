using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    // EP-12 US-129: office note PDF listing onboarding enclosures on file for a JobApplication.
    // Reuses the EP-10 DocumentTemplate/PlaceholderSubstitution/QuestPDF engine exactly like
    // JoiningBookletService. Enclosure list is built the same way DocumentTrackingService already
    // aggregates OfferLetter/AppointmentLetter/JoiningBooklet - queried at generation time, not a
    // stored join table. "Verification summary" is intentionally excluded - EP-11 was dropped
    // entirely from this project.
    public class OfficeNoteService : IOfficeNoteService
    {
        private readonly IOfficeNoteRepository _officeNoteRepository;
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly IAppointmentLetterRepository _appointmentLetterRepository;
        private readonly IJoiningBookletRepository _joiningBookletRepository;
        private readonly IDocumentTemplateRepository _documentTemplateRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IPlaceholderSubstitutionService _placeholderSubstitutionService;
        private readonly IOfficeNotePdfGeneratorService _officeNotePdfGeneratorService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        private const string PdfStorageSubFolder = "documents/office-notes";

        public OfficeNoteService(
            IOfficeNoteRepository officeNoteRepository,
            IOfferLetterRepository offerLetterRepository,
            IAppointmentLetterRepository appointmentLetterRepository,
            IJoiningBookletRepository joiningBookletRepository,
            IDocumentTemplateRepository documentTemplateRepository,
            IJobApplicationRepository jobApplicationRepository,
            IPlaceholderSubstitutionService placeholderSubstitutionService,
            IOfficeNotePdfGeneratorService officeNotePdfGeneratorService,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _officeNoteRepository = officeNoteRepository;
            _offerLetterRepository = offerLetterRepository;
            _appointmentLetterRepository = appointmentLetterRepository;
            _joiningBookletRepository = joiningBookletRepository;
            _documentTemplateRepository = documentTemplateRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _placeholderSubstitutionService = placeholderSubstitutionService;
            _officeNotePdfGeneratorService = officeNotePdfGeneratorService;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<OfficeNoteEnclosuresResponse> GetEnclosuresAsync(long jobApplicationId)
        {
            var enclosures = await BuildEnclosureListAsync(jobApplicationId);
            return new OfficeNoteEnclosuresResponse
            {
                Enclosures = enclosures.Select(e => new OfficeNoteEnclosureItemResponse
                {
                    DocumentType = e.Type,
                    Exists = e.GeneratedAt.HasValue,
                    GeneratedAt = e.GeneratedAt,
                }).ToList(),
            };
        }

        public async Task<OfficeNoteResponse> GenerateAsync(OfficeNoteGenerateRequest request)
        {
            var template = await _documentTemplateRepository.GetByIdAsync(request.DocumentTemplateId)
                ?? throw new NotFoundException("DocumentTemplate", request.DocumentTemplateId);

            if (template.DocumentType != DocumentTypeEnum.OfficeNote)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId),
                        "The selected template is not an OfficeNote-type DocumentTemplate.")
                });

            if (!template.IsActive)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.DocumentTemplateId), "The selected template is inactive.")
                });

            var jobApplication = await _jobApplicationRepository.GetByIdWithIncludeAsync(
                a => a.JobApplicationId == request.JobApplicationId,
                a => a.JobPosting)
                ?? throw new NotFoundException("JobApplication", request.JobApplicationId);

            var enclosures = await BuildEnclosureListAsync(request.JobApplicationId);
            var existingEnclosures = enclosures.Where(e => e.GeneratedAt.HasValue).ToList();
            if (existingEnclosures.Count == 0)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.JobApplicationId),
                        "No enclosures exist yet for this job application - generate at least an offer letter first.")
                });

            var enclosuresSummary = string.Join(", ", existingEnclosures.Select(e => e.DisplayName));

            var placeholderValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = jobApplication.CandidateName,
                ["JobTitle"] = jobApplication.JobPosting?.Title ?? string.Empty,
                ["EnclosureList"] = string.Join('\n', existingEnclosures.Select(e => $"- {e.DisplayName}")),
                ["Remarks"] = request.Remarks ?? string.Empty,
                ["GeneratedDate"] = DateTime.UtcNow.ToString("dd MMM yyyy"),
            };
            var renderedBody = _placeholderSubstitutionService.Render(template.Body, placeholderValues);

            var pdfBytes = _officeNotePdfGeneratorService.Generate(template.Name, jobApplication.CandidateName, renderedBody);

            using var pdfStream = new MemoryStream(pdfBytes);
            var (_, filePath) = await _fileStorageService.SaveAsync(pdfStream, "office-note.pdf", PdfStorageSubFolder);

            var entity = new OfficeNote
            {
                JobApplicationId = request.JobApplicationId,
                DocumentTemplateId = request.DocumentTemplateId,
                Remarks = request.Remarks,
                EnclosuresSummary = enclosuresSummary,
                RenderedBody = renderedBody,
                GeneratedPdfPath = filePath,
                GeneratedAt = DateTime.UtcNow,
            };

            await _officeNoteRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            entity.JobApplication = jobApplication;
            entity.DocumentTemplate = template;

            return entity.ToResponse();
        }

        public async Task<List<OfficeNoteResponse>> GetAllAsync(long? jobApplicationId)
        {
            var entities = await _officeNoteRepository.GetAllOrderedAsync(jobApplicationId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<OfficeNoteResponse> GetByIdAsync(long officeNoteId)
        {
            var entity = await _officeNoteRepository.GetByIdWithDetailsAsync(officeNoteId)
                ?? throw new NotFoundException("OfficeNote", officeNoteId);

            return entity.ToResponse();
        }

        private async Task<List<(DocumentTypeEnum Type, string DisplayName, DateTime? GeneratedAt)>> BuildEnclosureListAsync(long jobApplicationId)
        {
            var offerLetters = await _offerLetterRepository.GetAllOrderedAsync(jobApplicationId);
            var appointmentLetters = await _appointmentLetterRepository.GetAllOrderedAsync(jobApplicationId);
            var joiningBooklets = await _joiningBookletRepository.GetAllOrderedAsync(jobApplicationId);

            return new List<(DocumentTypeEnum, string, DateTime?)>
            {
                (DocumentTypeEnum.OfferLetter, "Offer Letter", offerLetters.FirstOrDefault()?.GeneratedAt),
                (DocumentTypeEnum.AppointmentLetter, "Appointment Letter", appointmentLetters.FirstOrDefault()?.GeneratedAt),
                (DocumentTypeEnum.JoiningBooklet, "Joining Booklet", joiningBooklets.FirstOrDefault()?.GeneratedAt),
            };
        }
    }
}
