using MediatR;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamScoreBulkUpload
{
    public class ExamScoreBulkUploadCommand : IRequest<ExamScoreBulkUploadResponse>
    {
        public long ExamId { get; set; }
        public IFormFile File { get; set; }

        public ExamScoreBulkUploadCommand(long examId, IFormFile file)
        {
            ExamId = examId;
            File = file;
        }
    }
}
