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
        private readonly ICvPdfGeneratorService _cvPdfGeneratorService;

        public CvBankCvDownloadHandler(ICandidateProfileRepository candidateProfileRepository, ICvPdfGeneratorService cvPdfGeneratorService)
        {
            _candidateProfileRepository = candidateProfileRepository;
            _cvPdfGeneratorService = cvPdfGeneratorService;
        }

        public async Task<CvBankCvFileResponse> Handle(CvBankCvDownloadQuery query, CancellationToken cancellationToken)
        {
            var profiles = await _candidateProfileRepository.GetByIdsWithDetailsAsync(new[] { query.CandidateProfileId });
            var profile = profiles.FirstOrDefault() ?? throw new NotFoundException("CandidateProfile", query.CandidateProfileId);

            var pdfBytes = _cvPdfGeneratorService.Generate(profile);

            return new CvBankCvFileResponse
            {
                Content = pdfBytes,
                ContentType = "application/pdf",
                FileName = CvFileNaming.ToPdfFileName(profile.FullName, profile.CandidateProfileId)
            };
        }
    }
}
