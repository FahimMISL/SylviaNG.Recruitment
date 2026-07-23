using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamVenueService : IExamVenueService
    {
        private readonly IExamVenueRepository _examVenueRepository;
        private readonly IExamRoomRepository _examRoomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamVenueService(IExamVenueRepository examVenueRepository, IExamRoomRepository examRoomRepository, IUnitOfWork unitOfWork)
        {
            _examVenueRepository = examVenueRepository;
            _examRoomRepository = examRoomRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExamVenueCreateRequest request)
        {
            var exists = await _examVenueRepository.ExistsByNameAsync(request.VenueName);
            if (exists)
                throw new DuplicateException("ExamVenue", "VenueName", request.VenueName);

            var entity = request.ToEntity();

            await _examVenueRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ExamVenueId;
        }

        public async Task UpdateAsync(long examVenueId, ExamVenueUpdateRequest request)
        {
            var entity = await _examVenueRepository.GetByIdAsync(examVenueId)
                ?? throw new NotFoundException("ExamVenue", examVenueId);

            var nameTaken = await _examVenueRepository.ExistsByNameAsync(request.VenueName, examVenueId);
            if (nameTaken)
                throw new DuplicateException("ExamVenue", "VenueName", request.VenueName);

            entity.VenueName = request.VenueName;
            entity.Location = request.Location;

            _examVenueRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(long examVenueId, bool isActive)
        {
            var entity = await _examVenueRepository.GetByIdAsync(examVenueId)
                ?? throw new NotFoundException("ExamVenue", examVenueId);

            entity.IsActive = isActive;
            _examVenueRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ExamVenueResponse> GetByIdAsync(long examVenueId)
        {
            var entity = await _examVenueRepository.GetByIdAsync(examVenueId)
                ?? throw new NotFoundException("ExamVenue", examVenueId);

            var roomCount = await _examRoomRepository.GetRoomCountByVenueIdAsync(examVenueId);
            return entity.ToResponse(roomCount);
        }

        public async Task<List<ExamVenueResponse>> GetAllAsync()
        {
            var entities = await _examVenueRepository.GetAllWithRoomsAsync();
            return entities.Select(e => e.ToResponse(e.Rooms.Count)).ToList();
        }

        public async Task<List<ExamVenueLookupResponse>> GetActiveLookupAsync()
        {
            var entities = await _examVenueRepository.GetActiveAsync();
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }
    }
}
