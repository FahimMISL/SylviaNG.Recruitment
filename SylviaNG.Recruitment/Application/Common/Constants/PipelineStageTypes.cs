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
            "Rejected",
            "Custom / General"
        };

        /// <summary>
        /// Stage types that represent post-decision steps (offer/onboarding), not part of the
        /// selection evaluation itself. Used to gate "Recommend for Final Selection" - every
        /// OTHER mandatory stage must be Completed first, since the recommendation is the
        /// decision that offer/onboarding are supposed to follow, not precede.
        /// </summary>
        public static readonly string[] PostDecision = { "Offer", "Joining", "Onboarding" };
    }
}
