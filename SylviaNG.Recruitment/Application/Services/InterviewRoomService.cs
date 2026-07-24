using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.InterviewRooms.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewRoomService : IInterviewRoomService
    {
        private readonly IInterviewRoomRepository _interviewRoomRepository;
        private readonly IInterviewVenueRepository _interviewVenueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewRoomService(IInterviewRoomRepository interviewRoomRepository, IInterviewVenueRepository interviewVenueRepository, IUnitOfWork unitOfWork)
        {
            _interviewRoomRepository = interviewRoomRepository;
            _interviewVenueRepository = interviewVenueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(long interviewVenueId, InterviewRoomCreateRequest request)
        {
            var venue = await _interviewVenueRepository.GetByIdAsync(interviewVenueId)
                ?? throw new NotFoundException("InterviewVenue", interviewVenueId);

            var exists = await _interviewRoomRepository.ExistsByNameAsync(interviewVenueId, request.RoomName);
            if (exists)
                throw new DuplicateException("InterviewRoom", "RoomName", request.RoomName);

            var entity = request.ToEntity(interviewVenueId);

            await _interviewRoomRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.InterviewRoomId;
        }

        public async Task UpdateAsync(long interviewRoomId, InterviewRoomUpdateRequest request)
        {
            var entity = await _interviewRoomRepository.GetByIdAsync(interviewRoomId)
                ?? throw new NotFoundException("InterviewRoom", interviewRoomId);

            var nameTaken = await _interviewRoomRepository.ExistsByNameAsync(entity.InterviewVenueId, request.RoomName, interviewRoomId);
            if (nameTaken)
                throw new DuplicateException("InterviewRoom", "RoomName", request.RoomName);

            entity.RoomName = request.RoomName;
            entity.Capacity = request.Capacity;

            _interviewRoomRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(long interviewRoomId, bool isActive)
        {
            var entity = await _interviewRoomRepository.GetByIdAsync(interviewRoomId)
                ?? throw new NotFoundException("InterviewRoom", interviewRoomId);

            entity.IsActive = isActive;
            _interviewRoomRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<InterviewRoomResponse> GetByIdAsync(long interviewRoomId)
        {
            var entity = await _interviewRoomRepository.GetByIdAsync(interviewRoomId)
                ?? throw new NotFoundException("InterviewRoom", interviewRoomId);

            return entity.ToResponse();
        }

        public async Task<List<InterviewRoomResponse>> GetAllByVenueIdAsync(long interviewVenueId)
        {
            var entities = await _interviewRoomRepository.GetAllByVenueIdAsync(interviewVenueId);
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
