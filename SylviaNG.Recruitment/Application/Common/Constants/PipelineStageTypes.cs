namespace SylviaNG.Recruitment.Application.Common.Constants
{
    /// <summary>
    /// Suggested stage types surfaced to admins in the pipeline builder UI. This is not an
    /// enforced enum — PipelineStage.StageType is free text so admins can define custom
    /// stage types without a code change.
    /// </summary>
    public static class PipelineStageTypes
    {
        public static readonly string[] Suggested =
        {
            "Application",
            "CvScreening",
            "ResumeReview",
            "PhoneScreening",
            "OnlineTest",
            "CodingTest",
            "TechnicalAssessment",
            "WrittenTest",
            "AptitudeTest",
            "PsychometricTest",
            "PracticalAssessment",
            "Assignment",
            "PortfolioReview",
            "CaseStudy",
            "Presentation",
            "GroupDiscussion",
            "TechnicalInterview",
            "FunctionalInterview",
            "ManagerInterview",
            "BehavioralInterview",
            "HrInterview",
            "PanelInterview",
            "ExecutiveInterview",
            "MedicalExamination",
            "ReferenceCheck",
            "BackgroundVerification",
            "SalaryNegotiation",
            "Offer",
            "Joining",
            "Onboarding",
            "Rejected"
        };
    }
}
