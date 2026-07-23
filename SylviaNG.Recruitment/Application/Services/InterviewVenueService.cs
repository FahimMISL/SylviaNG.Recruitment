using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewVenueService : IInterviewVenueService
    {
        private readonly IInterviewVenueRepository _interviewVenueRepository;
        private readonly IInterviewRoomRepository _interviewRoomRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewVenueService(IInterviewVenueRepository interviewVenueRepository, IInterviewRoomRepository interviewRoomRepository, IUnitOfWork unitOfWork)
        {
            _interviewVenueRepository = interviewVenueRepository;
            _interviewRoomRepository = interviewRoomRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(InterviewVenueCreateRequest request)
        {
            var exists = await _interviewVenueRepository.ExistsByNameAsync(request.VenueName);
            if (exists)
                throw new DuplicateException("InterviewVenue", "VenueName", request.VenueName);

            var entity = request.ToEntity();

            await _interviewVenueRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.InterviewVenueId;
        }

        public async Task UpdateAsync(long interviewVenueId, InterviewVenueUpdateRequest request)
        {
            var entity = await _interviewVenueRepository.GetByIdAsync(interviewVenueId)
                ?? throw new NotFoundException("InterviewVenue", interviewVenueId);

            var nameTaken = await _interviewVenueRepository.ExistsByNameAsync(request.VenueName, interviewVenueId);
            if (nameTaken)
                throw new DuplicateException("InterviewVenue", "VenueName", request.VenueName);

            entity.VenueName = request.VenueName;
            entity.Location = request.Location;

            _interviewVenueRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(long interviewVenueId, bool isActive)
        {
            var entity = await _interviewVenueRepository.GetByIdAsync(interviewVenueId)
                ?? throw new NotFoundException("InterviewVenue", interviewVenueId);

            entity.IsActive = isActive;
            _interviewVenueRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<InterviewVenueResponse> GetByIdAsync(long interviewVenueId)
        {
            var entity = await _interviewVenueRepository.GetByIdAsync(interviewVenueId)
                ?? throw new NotFoundException("InterviewVenue", interviewVenueId);

            var roomCount = await _interviewRoomRepository.GetRoomCountByVenueIdAsync(interviewVenueId);
            return entity.ToResponse(roomCount);
        }

        public async Task<List<InterviewVenueResponse>> GetAllAsync()
        {
            var entities = await _interviewVenueRepository.GetAllWithRoomsAsync();
            return entities.Select(e => e.ToResponse(e.Rooms.Count)).ToList();
        }

        public async Task<List<InterviewVenueLookupResponse>> GetActiveLookupAsync()
        {
            var entities = await _interviewVenueRepository.GetActiveAsync();
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }
    }
}
