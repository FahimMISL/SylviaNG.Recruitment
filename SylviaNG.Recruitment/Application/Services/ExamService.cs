using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IExamVenueRepository _examVenueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(
            IExamRepository examRepository,
            IJobPostingRepository jobPostingRepository,
            IExamVenueRepository examVenueRepository,
            IUnitOfWork unitOfWork)
        {
            _examRepository = examRepository;
            _jobPostingRepository = jobPostingRepository;
            _examVenueRepository = examVenueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExamCreateRequest request)
        {
            var jobPosting = await _jobPostingRepository.GetByIdAsync(request.JobPostingId)
                ?? throw new NotFoundException("JobPosting", request.JobPostingId);

            if (request.ExamType == ExamTypeEnum.InPerson)
            {
                if (!request.ExamVenueId.HasValue)
                    throw new InvalidStatusTransitionException("ExamVenueId is required for an in-person exam.");

                _ = await _examVenueRepository.GetByIdAsync(request.ExamVenueId.Value)
                    ?? throw new NotFoundException("ExamVenue", request.ExamVenueId.Value);
            }

            if (request.ExamType == ExamTypeEnum.Online)
            {
                if (!request.QuestionGroupId.HasValue)
                    throw new InvalidStatusTransitionException("QuestionGroupId is required for an online exam.");
            }

            var entity = request.ToEntity();

            await _examRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ExamId;
        }

        public async Task<PagedResult<ExamResponse>> GetPagedAsync(PagedRequest request, long? jobPostingId, ExamTypeEnum? examType, bool? isActive)
        {
            var paged = await _examRepository.GetPagedAsync(request, jobPostingId, examType, isActive);

            return new PagedResult<ExamResponse>
            {
                Data = paged.Data.Select(e => e.ToResponse()).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<ExamResponse> GetByIdAsync(long examId)
        {
            var entity = await _examRepository.GetByIdWithDetailsAsync(examId)
                ?? throw new NotFoundException("Exam", examId);

            return entity.ToResponse();
        }
    }
}
