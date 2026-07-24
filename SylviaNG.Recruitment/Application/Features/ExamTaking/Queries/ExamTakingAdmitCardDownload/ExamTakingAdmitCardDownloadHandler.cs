using MediatR;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Queries.ExamTakingAdmitCardDownload
{
    /// <summary>Candidate self-service admit-card download (US-057 AC4) - same PDF content as
    /// the HR-facing download, but ownership-checked against the calling candidate's own
    /// CandidateProfileId, same pattern as ExamTakingService.GetOwnedEnrollmentAsync.</summary>
    public class ExamTakingAdmitCardDownloadHandler : IRequestHandler<ExamTakingAdmitCardDownloadQuery, ExamFileResponse>
    {
        private readonly IExamEnrollmentRepository _examEnrollmentRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IAdmitCardPdfGeneratorService _admitCardPdfGeneratorService;

        public ExamTakingAdmitCardDownloadHandler(
            IExamEnrollmentRepository examEnrollmentRepository,
            ICurrentCandidateService currentCandidateService,
            IAdmitCardPdfGeneratorService admitCardPdfGeneratorService)
        {
            _examEnrollmentRepository = examEnrollmentRepository;
            _currentCandidateService = currentCandidateService;
            _admitCardPdfGeneratorService = admitCardPdfGeneratorService;
        }

        public async Task<ExamFileResponse> Handle(ExamTakingAdmitCardDownloadQuery query, CancellationToken cancellationToken)
        {
            var enrollment = await _examEnrollmentRepository.GetByIdWithDetailsAsync(query.ExamEnrollmentId)
                ?? throw new NotFoundException("ExamEnrollment", query.ExamEnrollmentId);

            var candidateProfileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            if (enrollment.JobApplication.CandidateProfileId != candidateProfileId)
                throw new ForbiddenException("This exam enrollment does not belong to you.");

            var content = _admitCardPdfGeneratorService.Generate(enrollment, enrollment.Exam, enrollment.JobApplication);

            return new ExamFileResponse
            {
                Content = content,
                FileName = $"Admit-Card-{enrollment.JobApplicationId}.pdf",
                ContentType = "application/pdf"
            };
        }
    }
}
