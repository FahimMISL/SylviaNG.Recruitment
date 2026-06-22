using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintCreate
{
    public class CandidateComplaintCreateValidator : AbstractValidator<CandidateComplaintCreateCommand>
    {
        public CandidateComplaintCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId)
                .GreaterThan(0).WithMessage("CandidateId is required.");

            RuleFor(x => x.Request.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(200).WithMessage("Category must not exceed 200 characters.");

            RuleFor(x => x.Request.Description)
                .NotEmpty().WithMessage("Description is required.");
        }
    }
}
