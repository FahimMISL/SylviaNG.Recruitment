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
        private readonly IQuestionGroupRepository _questionGroupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(
            IExamRepository examRepository,
            IJobPostingRepository jobPostingRepository,
            IExamVenueRepository examVenueRepository,
            IQuestionGroupRepository questionGroupRepository,
            IUnitOfWork unitOfWork)
        {
            _examRepository = examRepository;
            _jobPostingRepository = jobPostingRepository;
            _examVenueRepository = examVenueRepository;
            _questionGroupRepository = questionGroupRepository;
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

            var questionGroupIds = new List<long>();
            if (request.ExamType == ExamTypeEnum.Online)
            {
                if (request.QuestionGroupIds == null || request.QuestionGroupIds.Count == 0)
                    throw new InvalidStatusTransitionException("At least one question group is required for an online exam.");

                questionGroupIds = request.QuestionGroupIds.Distinct().ToList();
                var existingGroups = await _questionGroupRepository.GetByIdsAsync(questionGroupIds);
                var missingIds = questionGroupIds.Except(existingGroups.Select(g => g.QuestionGroupId)).ToList();
                if (missingIds.Count > 0)
                    throw new NotFoundException("QuestionGroup", missingIds.First());
            }

            var entity = request.ToEntity();
            foreach (var questionGroupId in questionGroupIds)
                entity.QuestionGroupLinks.Add(new Domain.Entities.ExamQuestionGroup { QuestionGroupId = questionGroupId });

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
