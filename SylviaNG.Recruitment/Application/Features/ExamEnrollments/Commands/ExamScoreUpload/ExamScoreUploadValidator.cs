using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamScoreUpload
{
    public class ExamScoreUploadValidator : AbstractValidator<ExamScoreUploadCommand>
    {
        public ExamScoreUploadValidator()
        {
            RuleFor(x => x.ExamEnrollmentId)
                .GreaterThan(0).WithMessage("ExamEnrollmentId is required.");

            RuleFor(x => x.Score)
                .GreaterThanOrEqualTo(0).WithMessage("Score must not be negative.");
        }
    }
}
