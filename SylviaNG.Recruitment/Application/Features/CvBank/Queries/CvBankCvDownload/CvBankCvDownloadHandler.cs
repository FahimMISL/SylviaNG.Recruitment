using MediatR;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankCvDownload
{
    public class CvBankCvDownloadHandler : IRequestHandler<CvBankCvDownloadQuery, CvBankCvFileResponse>
    {
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly ICvPdfGeneratorService _cvPdfGeneratorService;

        public CvBankCvDownloadHandler(
            ICandidateProfileRepository candidateProfileRepository,
            IJobApplicationRepository jobApplicationRepository,
            ICvPdfGeneratorService cvPdfGeneratorService)
        {
            _candidateProfileRepository = candidateProfileRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _cvPdfGeneratorService = cvPdfGeneratorService;
        }

        public async Task<CvBankCvFileResponse> Handle(CvBankCvDownloadQuery query, CancellationToken cancellationToken)
        {
            var profiles = await _candidateProfileRepository.GetByIdsWithDetailsAsync(new[] { query.CandidateProfileId });
            var profile = profiles.FirstOrDefault() ?? throw new NotFoundException("CandidateProfile", query.CandidateProfileId);

            // Critical fix: CandidateProfile carries no CompanyId of its own (candidates apply
            // across companies by design) - same anchor CvBankSearchHandler already uses: a
            // candidate is only visible to the caller's company if they have at least one
            // JobApplication row here, and JobApplication IS company-scoped, so this check is
            // automatically restricted to the caller's own company already.
            var hasApplicationInCallerCompany = (await _jobApplicationRepository
                .FindAsync(a => a.CandidateProfileId == query.CandidateProfileId))
                .Any();
            if (!hasApplicationInCallerCompany)
                throw new NotFoundException("CandidateProfile", query.CandidateProfileId);

            var pdfBytes = await _cvPdfGeneratorService.Generate(profile);

            return new CvBankCvFileResponse
            {
                Content = pdfBytes,
                ContentType = "application/pdf",
                FileName = CvFileNaming.ToPdfFileName(profile.FullName, profile.CandidateProfileId)
            };
        }
    }
}
