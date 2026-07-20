using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamHallService : IExamHallService
    {
        private readonly IExamHallRepository _examHallRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamHallService(IExamHallRepository examHallRepository, IUnitOfWork unitOfWork)
        {
            _examHallRepository = examHallRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExamHallCreateRequest request)
        {
            var exists = await _examHallRepository.ExistsByNameAsync(request.HallName);
            if (exists)
                throw new DuplicateException("ExamHall", "HallName", request.HallName);

            var entity = request.ToEntity();
            await ValidateInvigilatorIdsAsync(entity.Invigilators);

            await _examHallRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ExamHallId;
        }

        public async Task UpdateAsync(long examHallId, ExamHallUpdateRequest request)
        {
            var entity = await _examHallRepository.GetByIdWithInvigilatorsAsync(examHallId)
                ?? throw new NotFoundException("ExamHall", examHallId);

            var nameTaken = await _examHallRepository.ExistsByNameAsync(request.HallName, examHallId);
            if (nameTaken)
                throw new DuplicateException("ExamHall", "HallName", request.HallName);

            entity.HallName = request.HallName;
            entity.Location = request.Location;
            entity.TotalCapacity = request.TotalCapacity;
            entity.NotifyInvigilatorsOnAssign = request.NotifyInvigilatorsOnAssign;

            // Whole-collection replace, same convention as HiringPipelineService stage/interviewer updates.
            var newInvigilators = request.InvigilatorEmployeeIds
                .Distinct()
                .Select(employeeId => new ExamHallInvigilator { EmployeeId = employeeId })
                .ToList();
            await ValidateInvigilatorIdsAsync(newInvigilators);

            entity.Invigilators.Clear();
            foreach (var invigilator in newInvigilators)
            {
                entity.Invigilators.Add(invigilator);
            }

            _examHallRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(long examHallId, bool isActive)
        {
            var entity = await _examHallRepository.GetByIdAsync(examHallId)
                ?? throw new NotFoundException("ExamHall", examHallId);

            entity.IsActive = isActive;
            _examHallRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ExamHallResponse> GetByIdAsync(long examHallId)
        {
            var entity = await _examHallRepository.GetByIdWithInvigilatorsAsync(examHallId)
                ?? throw new NotFoundException("ExamHall", examHallId);

            return entity.ToResponse();
        }

        public async Task<List<ExamHallResponse>> GetAllAsync()
        {
            var entities = await _examHallRepository.GetAllWithInvigilatorsAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<List<ExamHallLookupResponse>> GetActiveLookupAsync()
        {
            var entities = await _examHallRepository.GetActiveAsync();
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }

        private async Task ValidateInvigilatorIdsAsync(IEnumerable<ExamHallInvigilator> invigilators)
        {
            var requestedIds = invigilators.Select(i => i.EmployeeId).Distinct().ToList();
            if (requestedIds.Count == 0)
                return;

            var existingIds = await _examHallRepository.GetExistingEmployeeIdsAsync(requestedIds);
            var missingIds = requestedIds.Where(id => !existingIds.Contains(id)).ToList();

            if (missingIds.Count > 0)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        "InvigilatorEmployeeIds",
                        $"Unknown employee id(s): {string.Join(", ", missingIds)}")
                });
            }
        }
    }
}
