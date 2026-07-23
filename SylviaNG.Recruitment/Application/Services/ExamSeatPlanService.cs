using ClosedXML.Excel;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Full-regenerate seat-plan allocator (US-056): every call to GenerateAsync clears and
    /// rebuilds every enrollment's room/seat assignment from scratch, walking active rooms in
    /// RoomName order and filling each to Capacity before moving to the next.
    /// </summary>
    public class ExamSeatPlanService : IExamSeatPlanService
    {
        private readonly IExamRepository _examRepository;
        private readonly IExamEnrollmentRepository _examEnrollmentRepository;
        private readonly IExamRoomRepository _examRoomRepository;
        private readonly ISeatPlanPdfGeneratorService _seatPlanPdfGeneratorService;
        private readonly IAdmitCardPdfGeneratorService _admitCardPdfGeneratorService;
        private readonly IUnitOfWork _unitOfWork;

        public ExamSeatPlanService(
            IExamRepository examRepository,
            IExamEnrollmentRepository examEnrollmentRepository,
            IExamRoomRepository examRoomRepository,
            ISeatPlanPdfGeneratorService seatPlanPdfGeneratorService,
            IAdmitCardPdfGeneratorService admitCardPdfGeneratorService,
            IUnitOfWork unitOfWork)
        {
            _examRepository = examRepository;
            _examEnrollmentRepository = examEnrollmentRepository;
            _examRoomRepository = examRoomRepository;
            _seatPlanPdfGeneratorService = seatPlanPdfGeneratorService;
            _admitCardPdfGeneratorService = admitCardPdfGeneratorService;
            _unitOfWork = unitOfWork;
        }

        public async Task GenerateAsync(long examId)
        {
            var exam = await _examRepository.GetByIdWithDetailsAsync(examId)
                ?? throw new NotFoundException("Exam", examId);

            if (exam.ExamType != ExamTypeEnum.InPerson || !exam.ExamVenueId.HasValue)
                throw new InvalidStatusTransitionException($"Exam {examId} is not an in-person exam with a venue - cannot generate a seat plan.");

            var rooms = await _examRoomRepository.GetActiveByVenueIdAsync(exam.ExamVenueId.Value);
            var enrollments = await _examEnrollmentRepository.GetByExamIdAsync(examId);

            var totalCapacity = rooms.Sum(r => r.Capacity);
            if (enrollments.Count > totalCapacity)
                throw new InvalidStatusTransitionException(
                    $"Not enough seat capacity: {enrollments.Count} enrolled, {totalCapacity} available.");

            foreach (var enrollment in enrollments)
            {
                enrollment.ExamRoomId = null;
                enrollment.SeatNumber = null;
            }

            var orderedRooms = rooms.OrderBy(r => r.RoomName, StringComparer.OrdinalIgnoreCase).ToList();
            var enrollmentQueue = new Queue<ExamEnrollment>(enrollments);

            foreach (var room in orderedRooms)
            {
                var seq = 1;
                while (seq <= room.Capacity && enrollmentQueue.Count > 0)
                {
                    var enrollment = enrollmentQueue.Dequeue();
                    enrollment.ExamRoomId = room.ExamRoomId;
                    enrollment.SeatNumber = $"{room.RoomName}-{seq:000}";
                    seq++;
                }

                if (enrollmentQueue.Count == 0)
                    break;
            }

            foreach (var enrollment in enrollments)
                _examEnrollmentRepository.Update(enrollment);

            exam.SeatPlanGeneratedAt = DateTime.UtcNow;
            _examRepository.Update(exam);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<(byte[] Content, string FileName)> GeneratePdfAsync(long examId)
        {
            var exam = await _examRepository.GetByIdWithDetailsAsync(examId)
                ?? throw new NotFoundException("Exam", examId);

            var enrollments = await _examEnrollmentRepository.GetByExamIdAsync(examId);

            var content = _seatPlanPdfGeneratorService.Generate(exam, enrollments);
            var fileName = $"Seat-Plan-{examId}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.pdf";

            return (content, fileName);
        }

        public async Task<(byte[] Content, string FileName)> GenerateExcelAsync(long examId)
        {
            var exam = await _examRepository.GetByIdWithDetailsAsync(examId)
                ?? throw new NotFoundException("Exam", examId);

            var enrollments = await _examEnrollmentRepository.GetByExamIdAsync(examId);

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Seat Plan");

            var headers = new[] { "Room", "Seat Number", "Candidate Name", "Application ID" };
            for (var column = 0; column < headers.Length; column++)
                sheet.Cell(1, column + 1).Value = headers[column];
            sheet.Row(1).Style.Font.Bold = true;

            var rowIndex = 2;
            foreach (var enrollment in enrollments
                .OrderBy(e => e.ExamRoom?.RoomName ?? "Unassigned", StringComparer.OrdinalIgnoreCase)
                .ThenBy(e => e.SeatNumber, StringComparer.OrdinalIgnoreCase))
            {
                sheet.Cell(rowIndex, 1).Value = enrollment.ExamRoom?.RoomName ?? "Unassigned";
                sheet.Cell(rowIndex, 2).Value = enrollment.SeatNumber ?? string.Empty;
                sheet.Cell(rowIndex, 3).Value = enrollment.JobApplication?.CandidateName ?? string.Empty;
                sheet.Cell(rowIndex, 4).Value = enrollment.JobApplicationId;
                rowIndex++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var fileName = $"Seat-Plan-{examId}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";

            return (stream.ToArray(), fileName);
        }

        public async Task<(byte[] Content, string FileName)> GenerateAdmitCardPdfAsync(long examEnrollmentId)
        {
            var enrollment = await _examEnrollmentRepository.GetByIdWithDetailsAsync(examEnrollmentId)
                ?? throw new NotFoundException("ExamEnrollment", examEnrollmentId);

            var content = _admitCardPdfGeneratorService.Generate(enrollment, enrollment.Exam, enrollment.JobApplication);
            var fileName = $"Admit-Card-{enrollment.JobApplicationId}.pdf";

            return (content, fileName);
        }
    }
}
