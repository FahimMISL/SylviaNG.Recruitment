using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamRooms.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamRoomService : IExamRoomService
    {
        private readonly IExamRoomRepository _examRoomRepository;
        private readonly IExamVenueRepository _examVenueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamRoomService(IExamRoomRepository examRoomRepository, IExamVenueRepository examVenueRepository, IUnitOfWork unitOfWork)
        {
            _examRoomRepository = examRoomRepository;
            _examVenueRepository = examVenueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(long examVenueId, ExamRoomCreateRequest request)
        {
            var venue = await _examVenueRepository.GetByIdAsync(examVenueId)
                ?? throw new NotFoundException("ExamVenue", examVenueId);

            var exists = await _examRoomRepository.ExistsByNameAsync(examVenueId, request.RoomName);
            if (exists)
                throw new DuplicateException("ExamRoom", "RoomName", request.RoomName);

            var entity = request.ToEntity(examVenueId);
            await ValidateInvigilatorIdsAsync(entity.Invigilators);

            await _examRoomRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ExamRoomId;
        }

        public async Task UpdateAsync(long examRoomId, ExamRoomUpdateRequest request)
        {
            var entity = await _examRoomRepository.GetByIdWithInvigilatorsAsync(examRoomId)
                ?? throw new NotFoundException("ExamRoom", examRoomId);

            var nameTaken = await _examRoomRepository.ExistsByNameAsync(entity.ExamVenueId, request.RoomName, examRoomId);
            if (nameTaken)
                throw new DuplicateException("ExamRoom", "RoomName", request.RoomName);

            entity.RoomName = request.RoomName;
            entity.Capacity = request.Capacity;
            entity.NotifyInvigilatorsOnAssign = request.NotifyInvigilatorsOnAssign;

            // Whole-collection replace, same convention as HiringPipelineService stage/interviewer updates.
            var newInvigilators = request.InvigilatorEmployeeIds
                .Distinct()
                .Select(employeeId => new ExamRoomInvigilator { EmployeeId = employeeId })
                .ToList();
            await ValidateInvigilatorIdsAsync(newInvigilators);

            entity.Invigilators.Clear();
            foreach (var invigilator in newInvigilators)
            {
                entity.Invigilators.Add(invigilator);
            }

            _examRoomRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(long examRoomId, bool isActive)
        {
            var entity = await _examRoomRepository.GetByIdAsync(examRoomId)
                ?? throw new NotFoundException("ExamRoom", examRoomId);

            entity.IsActive = isActive;
            _examRoomRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ExamRoomResponse> GetByIdAsync(long examRoomId)
        {
            var entity = await _examRoomRepository.GetByIdWithInvigilatorsAsync(examRoomId)
                ?? throw new NotFoundException("ExamRoom", examRoomId);

            return entity.ToResponse();
        }

        public async Task<List<ExamRoomResponse>> GetAllByVenueIdAsync(long examVenueId)
        {
            var entities = await _examRoomRepository.GetAllByVenueIdAsync(examVenueId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        private async Task ValidateInvigilatorIdsAsync(IEnumerable<ExamRoomInvigilator> invigilators)
        {
            var requestedIds = invigilators.Select(i => i.EmployeeId).Distinct().ToList();
            if (requestedIds.Count == 0)
                return;

            var existingIds = await _examRoomRepository.GetExistingEmployeeIdsAsync(requestedIds);
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
