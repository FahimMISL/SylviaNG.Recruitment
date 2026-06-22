using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ExamSeatPlanMapper
    {
        public static ExamSeatPlan ToEntity(this ExamSeatPlanCreateRequest request)
        {
            return new ExamSeatPlan
            {
                ExamId = request.ExamId,
                ExamHallId = request.ExamHallId,
                ExamCandidateId = request.ExamCandidateId,
                RoomNumber = request.RoomNumber,
                SeatNumber = request.SeatNumber,
            };
        }

        public static void ApplyUpdate(this ExamSeatPlan entity, ExamSeatPlanUpdateRequest request)
        {
            if (request.ExamId.HasValue) entity.ExamId = request.ExamId.Value;
            if (request.ExamHallId.HasValue) entity.ExamHallId = request.ExamHallId.Value;
            if (request.ExamCandidateId.HasValue) entity.ExamCandidateId = request.ExamCandidateId.Value;
            if (request.RoomNumber is not null) entity.RoomNumber = request.RoomNumber;
            if (request.SeatNumber is not null) entity.SeatNumber = request.SeatNumber;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ExamSeatPlanResponse ToResponse(this ExamSeatPlan entity)
        {
            return new ExamSeatPlanResponse
            {
                ExamSeatPlanId = entity.ExamSeatPlanId,
                ExamId = entity.ExamId,
                ExamHallId = entity.ExamHallId,
                ExamCandidateId = entity.ExamCandidateId,
                RoomNumber = entity.RoomNumber,
                SeatNumber = entity.SeatNumber,
                IsActive = entity.IsActive,
            };
        }
    }
}
