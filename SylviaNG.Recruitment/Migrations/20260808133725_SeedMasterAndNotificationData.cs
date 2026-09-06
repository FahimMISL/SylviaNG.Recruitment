using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SylviaNG.Recruitment.Migrations
{
    /// <inheritdoc />
    public partial class SeedMasterAndNotificationData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SeedSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(DeleteSql);
        }

        // EP-09/EP-10 default master data + notification/document templates + event->template
        // mappings, captured from the reference environment. Guarded so it ONLY seeds an empty
        // database: re-running against a DB that already has notification templates is a no-op,
        // so this is safe to apply to existing databases as well as fresh ones.
        private const string SeedSql = """
DO $seed$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM public."NotificationTemplates") THEN
INSERT INTO public."DocumentTemplates" ("DocumentTemplateId", "DocumentType", "Code", "Name", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (1, 'OfferLetter', 'OFFER_LETTER_STANDARD', 'Standard Offer Letter', 'Dear {{CandidateName}},

We are pleased to offer you the position of {{Designation}} at Sylvia Ltd, following your application for {{JobTitle}}.

The terms of your offer are as follows:

  Designation        : {{Designation}}
  Annual Salary      : BDT {{OfferedSalary}}
  Joining Date       : {{JoiningDate}}
  Reporting Manager  : {{ReportingManager}}

This offer is valid until {{OfferValidityDate}}. Please review and respond through your candidate portal:
{{PortalLink}}

Kindly confirm your acceptance on or before the validity date above. Should you have any questions, reply to this letter or contact our HR team at {{CandidateEmail}}.

We look forward to welcoming you to the team.

Warm regards,
Human Resources
Sylvia Ltd', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplates" ("DocumentTemplateId", "DocumentType", "Code", "Name", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (2, 'AppointmentLetter', 'APPOINTMENT_LETTER_STANDARD', 'Standard Appointment Letter', 'Dear {{CandidateName}},

Further to your acceptance of our offer, we are pleased to confirm your appointment to the position of {{Designation}} at Sylvia Ltd.

Your date of joining is {{JoiningDate}}. Please report to the HR department on that date with the documents listed in your candidate portal.

View your appointment details here:
{{PortalLink}}

We look forward to a long and rewarding association.

Warm regards,
Human Resources
Sylvia Ltd', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplates" ("DocumentTemplateId", "DocumentType", "Code", "Name", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (3, 'JoiningBooklet', 'JOINING_BOOKLET_STANDARD', 'Standard Joining Booklet', 'Welcome, {{CandidateName}}!

We are delighted to have you join Sylvia Ltd as {{Designation}}, part of joining batch {{BatchLabel}}.

Your joining date is {{JoiningDate}}. This booklet covers your first-day checklist, company policies, and onboarding schedule.

Complete your pre-joining formalities in the candidate portal:
{{PortalLink}}

Warm regards,
Human Resources
Sylvia Ltd', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplates" ("DocumentTemplateId", "DocumentType", "Code", "Name", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (4, 'MedicalReferral', 'MEDICAL_REFERRAL_STANDARD', 'Standard Medical Referral', 'Dear {{CandidateName}},

As part of your pre-employment process (Reference: {{CandidateReference}}), you are required to complete a medical examination.

  Test Center    : {{MedicalTestCenter}}
  Required Tests : {{RequiredTests}}

Please carry this referral and a valid photo ID to the test center. Report your completion through the candidate portal:
{{PortalLink}}

Warm regards,
Human Resources
Sylvia Ltd', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplates" ("DocumentTemplateId", "DocumentType", "Code", "Name", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (5, 'TargetLetter', 'TARGET_LETTER_STANDARD', 'Standard Target Letter', 'Dear {{CandidateName}},

As {{Designation}}, the following performance targets have been set for your role.

Key Performance Indicators:
{{Kpis}}

Objectives:
{{Objectives}}

You can review these targets any time in your portal:
{{PortalLink}}

Warm regards,
Human Resources
Sylvia Ltd', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplates" ("DocumentTemplateId", "DocumentType", "Code", "Name", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (6, 'OfficeNote', 'OFFICE_NOTE_STANDARD', 'Standard Office Note', 'OFFICE NOTE                                    Date: {{GeneratedDate}}

Subject: Recruitment file — {{CandidateName}} ({{JobTitle}})

The recruitment file for the above candidate is placed for review. Enclosures attached:

{{EnclosureList}}

Remarks: {{Remarks}}

Submitted for approval.

Human Resources
Sylvia Ltd', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplates" ("DocumentTemplateId", "DocumentType", "Code", "Name", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (7, 'RejectionLetter', 'REJECTION_LETTER_STANDARD', 'Standard Rejection Letter', 'Dear Candidate,

Thank you for your interest in Sylvia Ltd and for the time invested in our recruitment process.

After careful consideration, we regret to inform you that we will not be moving forward with your application at this time. This decision does not reflect on your abilities, and we encourage you to apply for future openings that match your profile.

We wish you success in your career.

Warm regards,
Human Resources
Sylvia Ltd', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);


--
-- Data for Name: DocumentTemplateVersions; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."DocumentTemplateVersions" ("DocumentTemplateVersionId", "DocumentTemplateId", "VersionNumber", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (1, 1, 1, 'Dear {{CandidateName}},

We are pleased to offer you the position of {{Designation}} at Sylvia Ltd, following your application for {{JobTitle}}.

The terms of your offer are as follows:

  Designation        : {{Designation}}
  Annual Salary      : BDT {{OfferedSalary}}
  Joining Date       : {{JoiningDate}}
  Reporting Manager  : {{ReportingManager}}

This offer is valid until {{OfferValidityDate}}. Please review and respond through your candidate portal:
{{PortalLink}}

Kindly confirm your acceptance on or before the validity date above. Should you have any questions, reply to this letter or contact our HR team at {{CandidateEmail}}.

We look forward to welcoming you to the team.

Warm regards,
Human Resources
Sylvia Ltd', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplateVersions" ("DocumentTemplateVersionId", "DocumentTemplateId", "VersionNumber", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (2, 2, 1, 'Dear {{CandidateName}},

Further to your acceptance of our offer, we are pleased to confirm your appointment to the position of {{Designation}} at Sylvia Ltd.

Your date of joining is {{JoiningDate}}. Please report to the HR department on that date with the documents listed in your candidate portal.

View your appointment details here:
{{PortalLink}}

We look forward to a long and rewarding association.

Warm regards,
Human Resources
Sylvia Ltd', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplateVersions" ("DocumentTemplateVersionId", "DocumentTemplateId", "VersionNumber", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (3, 3, 1, 'Welcome, {{CandidateName}}!

We are delighted to have you join Sylvia Ltd as {{Designation}}, part of joining batch {{BatchLabel}}.

Your joining date is {{JoiningDate}}. This booklet covers your first-day checklist, company policies, and onboarding schedule.

Complete your pre-joining formalities in the candidate portal:
{{PortalLink}}

Warm regards,
Human Resources
Sylvia Ltd', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplateVersions" ("DocumentTemplateVersionId", "DocumentTemplateId", "VersionNumber", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (4, 4, 1, 'Dear {{CandidateName}},

As part of your pre-employment process (Reference: {{CandidateReference}}), you are required to complete a medical examination.

  Test Center    : {{MedicalTestCenter}}
  Required Tests : {{RequiredTests}}

Please carry this referral and a valid photo ID to the test center. Report your completion through the candidate portal:
{{PortalLink}}

Warm regards,
Human Resources
Sylvia Ltd', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplateVersions" ("DocumentTemplateVersionId", "DocumentTemplateId", "VersionNumber", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (5, 5, 1, 'Dear {{CandidateName}},

As {{Designation}}, the following performance targets have been set for your role.

Key Performance Indicators:
{{Kpis}}

Objectives:
{{Objectives}}

You can review these targets any time in your portal:
{{PortalLink}}

Warm regards,
Human Resources
Sylvia Ltd', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplateVersions" ("DocumentTemplateVersionId", "DocumentTemplateId", "VersionNumber", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (6, 6, 1, 'OFFICE NOTE                                    Date: {{GeneratedDate}}

Subject: Recruitment file — {{CandidateName}} ({{JobTitle}})

The recruitment file for the above candidate is placed for review. Enclosures attached:

{{EnclosureList}}

Remarks: {{Remarks}}

Submitted for approval.

Human Resources
Sylvia Ltd', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."DocumentTemplateVersions" ("DocumentTemplateVersionId", "DocumentTemplateId", "VersionNumber", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (7, 7, 1, 'Dear Candidate,

Thank you for your interest in Sylvia Ltd and for the time invested in our recruitment process.

After careful consideration, we regret to inform you that we will not be moving forward with your application at this time. This decision does not reflect on your abilities, and we encourage you to apply for future openings that match your profile.

We wish you success in your career.

Warm regards,
Human Resources
Sylvia Ltd', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);


--
-- Data for Name: NotificationTemplates; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (1, 'Email', 'EMAIL_APPLICATION_REJECTED', 'Application Rejected — Email', 'Update on your application for {{JobPostingTitle}}', 'Dear {{CandidateName}},

Thank you for applying for {{JobPostingTitle}}. After careful review, we will not be moving forward with your application at this time.

We appreciate your interest and encourage you to apply for future roles that match your profile.

Warm regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (2, 'Email', 'EMAIL_ACCOUNT_OTP', 'Account Verification OTP', 'Your verification code', 'Dear {{CandidateName}},

Your one-time verification code is:

    {{OtpCode}}

This code expires in {{ExpiryMinutes}} minutes. Do not share it with anyone.

If you did not request this, please ignore this email.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (3, 'Email', 'EMAIL_PASSWORD_RESET_OTP', 'Password Reset OTP', 'Password reset code', 'Dear {{CandidateName}},

We received a request to reset your password. Your reset code is:

    {{OtpCode}}

This code expires in {{ExpiryMinutes}} minutes. If you did not request a reset, ignore this email and your password stays unchanged.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (4, 'Email', 'EMAIL_CHANGE_OTP', 'Email Change Verification', 'Confirm your new email address', 'Hello,

Use this code to confirm your new email address:

    {{OtpCode}}

The code expires in {{ExpiryMinutes}} minutes. If you did not request this change, ignore this email.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (5, 'Email', 'EMAIL_APPLICATION_RECEIVED', 'Application Received — Candidate', 'We received your application for {{JobPostingTitle}}', 'Dear {{CandidateName}},

Thank you for applying for {{JobPostingTitle}}. Your application has been received and is currently marked as {{ApplicationStatus}}.

We will review it and keep you updated on next steps. You can track progress in your candidate portal.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (6, 'Email', 'EMAIL_APPLICATION_STATUS_CHANGED', 'Application Status Update', 'Update on your application for {{JobPostingTitle}}', 'Dear {{CandidateName}},

The status of your application for {{JobPostingTitle}} has changed from {{FromStatus}} to {{ToStatus}}.

Log in to your candidate portal for details and any required action.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (7, 'Email', 'EMAIL_INTERVIEW_SCHEDULED', 'Interview Scheduled', 'Your interview is scheduled', 'Dear {{CandidateName}},

Your interview has been scheduled.

  Date & Time : {{ScheduledStartAt}} to {{ScheduledEndAt}}
  {{LocationLabel}} : {{LocationValue}}

Please be available on time. If you have any conflict, contact us as early as possible.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (8, 'Email', 'EMAIL_INTERVIEW_RESCHEDULED', 'Interview Rescheduled', 'Your interview has been rescheduled', 'Dear {{CandidateName}},

Your interview has been rescheduled to a new time:

  Date & Time : {{ScheduledStartAt}} to {{ScheduledEndAt}}
  {{LocationLabel}} : {{LocationValue}}

Please update your calendar accordingly.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (9, 'Email', 'EMAIL_INTERVIEW_CANCELLED', 'Interview Cancelled', 'Your interview has been cancelled', 'Dear {{CandidateName}},

We regret to inform you that your scheduled interview has been cancelled.

Reason: {{CancellationReason}}

We will contact you if it is to be rescheduled.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (10, 'Email', 'EMAIL_EXAM_ENROLLED', 'Exam Enrollment', 'You are enrolled for {{ExamTitle}}', 'Dear {{CandidateName}},

You have been enrolled for the following exam:

  Exam     : {{ExamTitle}}
  Date     : {{ScheduledStartAt}}
  Duration : {{DurationMinutes}} minutes
  Venue    : {{VenueName}}, {{VenueLocation}}
  Seat No. : {{SeatNumber}}

Carry a valid photo ID. Further instructions will follow with your admit card.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (11, 'Email', 'EMAIL_ADMIT_CARD_ISSUED', 'Admit Card Issued', 'Your admit card for {{ExamTitle}}', 'Dear {{CandidateName}},

Your admit card for {{ExamTitle}} is now available.

  Date     : {{ScheduledStartAt}}
  Duration : {{DurationMinutes}} minutes
  Venue    : {{VenueName}}, {{VenueLocation}}
  Seat No. : {{SeatNumber}}

Download your admit card from the candidate portal and bring a printed copy with a valid photo ID.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (12, 'Email', 'EMAIL_OFFER_AVAILABLE', 'Offer Letter Available', 'Your offer letter for {{Designation}}', 'Dear {{CandidateName}},

Congratulations! We are pleased to offer you the position of {{Designation}}.

  Annual Salary     : {{OfferedSalary}}
  Joining Date      : {{JoiningDate}}
  Reporting Manager : {{ReportingManager}}
  Valid Until       : {{OfferValidityDate}}

Review and respond to your offer here:
{{PortalLink}}

Please accept or decline before the validity date.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (13, 'Email', 'EMAIL_APPOINTMENT_GENERATED', 'Appointment Letter Available', 'Your appointment letter is ready', 'Dear {{CandidateName}},

Your appointment letter for the position of {{Designation}} is now available. Your joining date is {{JoiningDate}}.

Download it here:
{{PortalLink}}

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (14, 'Email', 'EMAIL_JOINING_BOOKLET', 'Joining Booklet Available', 'Your joining booklet', 'Dear {{CandidateName}},

Welcome aboard as {{Designation}} (batch {{BatchLabel}}). Your joining date is {{JoiningDate}}.

Your joining booklet with onboarding details is available here:
{{PortalLink}}

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (15, 'Email', 'EMAIL_MEDICAL_REFERRAL', 'Medical Referral Available', 'Medical examination required', 'Dear {{CandidateName}},

As part of your pre-employment process (Ref: {{CandidateReference}}), a medical examination is required.

  Test Center    : {{MedicalTestCenter}}
  Required Tests : {{RequiredTests}}

Details and your referral letter are here:
{{PortalLink}}

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (16, 'Email', 'EMAIL_TARGET_LETTER', 'Target Letter Available', 'Your performance targets', 'Dear {{CandidateName}},

Your performance targets for the role of {{Designation}} are now available.

KPIs:
{{Kpis}}

Objectives:
{{Objectives}}

View full details here:
{{PortalLink}}

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (17, 'Email', 'EMAIL_PREBOARDING_REQUESTED', 'Pre-Boarding Form Requested', 'Complete your pre-boarding form', 'Dear {{CandidateName}},

Congratulations on being selected for {{Designation}} (joining {{JoiningDate}}). Before you join, please complete your pre-boarding form.

Fill it here:
{{PortalLink}}

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (18, 'Email', 'EMAIL_PREBOARDING_APPROVED', 'Pre-Boarding Approved', 'Your pre-boarding submission is approved', 'Dear {{CandidateName}},

Your pre-boarding submission has been reviewed and approved on {{ValidatedAt}}. No further action is needed at this stage.

We look forward to welcoming you.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (19, 'Email', 'EMAIL_PREBOARDING_CORRECTION', 'Pre-Boarding Correction Requested', 'Action needed on your pre-boarding submission', 'Dear {{CandidateName}},

Your pre-boarding submission needs some corrections before it can be approved.

Reviewer comment: {{Comment}}

Please log in and update the requested details.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (20, 'Email', 'EMAIL_HR_NEW_APPLICATION', 'New Application — HR Alert', 'New application for {{JobPostingTitle}}', 'A new candidate has applied.

  Candidate : {{CandidateName}}
  Position  : {{JobPostingTitle}}
  Status    : {{ApplicationStatus}}

Review it in the recruitment dashboard.', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (21, 'Email', 'EMAIL_HR_APPLICATION_WITHDRAWN', 'Application Withdrawn — HR Alert', 'Application withdrawn: {{CandidateName}}', 'A candidate has withdrawn their application.

  Candidate : {{CandidateName}}
  Position  : {{JobPostingTitle}}
  Status    : {{FromStatus}} -> {{ToStatus}}', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (22, 'Email', 'EMAIL_HR_OFFER_ACCEPTED', 'Offer Accepted — HR Alert', 'Offer accepted by {{CandidateName}}', 'Good news — an offer has been accepted.

  Candidate   : {{CandidateName}}
  Designation : {{Designation}}
  Accepted At : {{DecisionAt}}', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (23, 'Email', 'EMAIL_HR_OFFER_DECLINED', 'Offer Declined — HR Alert', 'Offer declined by {{CandidateName}}', 'An offer has been declined.

  Candidate   : {{CandidateName}}
  Designation : {{Designation}}
  Declined At : {{DecisionAt}}
  Reason      : {{DeclineReason}}', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (24, 'Email', 'EMAIL_HR_PREBOARDING_SUBMITTED', 'Pre-Boarding Submitted — HR Alert', 'Pre-boarding submitted by {{CandidateName}}', 'A candidate has submitted their pre-boarding form.

  Candidate    : {{CandidateName}}
  Submitted At : {{SubmittedAt}}

Please review and validate it.', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (25, 'Email', 'EMAIL_EXPORT_READY', 'Export Ready', 'Your export is ready', 'Your requested export has finished generating.

  File : {{FileName}}
  Rows : {{RowCount}}

Download it from the export section of the portal.', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (26, 'Email', 'EMAIL_EXPORT_FAILED', 'Export Failed', 'Your export failed', 'Your requested export could not be generated.

Reason: {{FailureReason}}

Please try again or contact support if the problem persists.', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (27, 'Email', 'EMAIL_PAYMENT_REQUIRED', 'Payment Required — Candidate', 'Complete your payment for {{JobPostingTitle}}', 'Dear {{CandidateName}},

Your application for {{JobPostingTitle}} has been received but is not yet complete. An application fee of {{FeeAmount}} is required to submit it.

Complete your payment here:
{{PaymentLink}}

Your application stays in "{{ApplicationStatus}}" until payment is confirmed. If you have already paid, please ignore this email.

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (28, 'Email', 'EMAIL_HR_PAYMENT_PENDING', 'Payment Pending — HR Alert', 'Application awaiting payment: {{CandidateName}}', 'An application is awaiting payment.

  Candidate : {{CandidateName}}
  Position  : {{JobPostingTitle}}
  Fee       : {{FeeAmount}}
  Status    : {{ApplicationStatus}}

It will proceed automatically once the candidate completes payment.', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplates" ("NotificationTemplateId", "Channel", "Code", "Name", "Subject", "Body", "IsActive", "CurrentVersionNumber", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (29, 'Email', 'EMAIL_EXAM_RESULT_PUBLISHED', 'Exam Result Published', 'Your result for {{ExamTitle}}', 'Dear {{CandidateName}},

Your result for {{ExamTitle}} has been published.

  Score  : {{Score}} / {{TotalMarks}}
  Pass Mark : {{PassMarks}}
  Result : {{ResultStatus}}

View full details in your portal:
{{PortalLink}}

Regards,
Human Resources', true, 1, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);


--
-- Data for Name: EventTemplateMappings; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (1, 'AccountCreatedOtp', 'Email', 'Candidate', 2, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (2, 'PasswordResetRequested', 'Email', 'Candidate', 3, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (3, 'EmailChangeRequested', 'Email', 'Candidate', 4, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (4, 'ApplicationSubmitted', 'Email', 'Candidate', 5, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (5, 'ApplicationSubmitted', 'Email', 'AdminHr', 20, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (6, 'ApplicationStatusChanged', 'Email', 'Candidate', 6, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (7, 'ApplicationWithdrawn', 'Email', 'AdminHr', 21, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (8, 'CandidateActionRequired', 'Email', 'Candidate', 27, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (9, 'CandidateActionRequired', 'Email', 'AdminHr', 28, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (10, 'InterviewScheduled', 'Email', 'Candidate', 7, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (11, 'InterviewRescheduled', 'Email', 'Candidate', 8, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (12, 'InterviewCancelled', 'Email', 'Candidate', 9, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (13, 'ExamEnrolled', 'Email', 'Candidate', 10, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (14, 'AdmitCardIssued', 'Email', 'Candidate', 11, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (15, 'ExamResultPublished', 'Email', 'Candidate', 29, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (16, 'OfferLetterAvailable', 'Email', 'Candidate', 12, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (17, 'OfferAccepted', 'Email', 'AdminHr', 22, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (18, 'OfferDeclined', 'Email', 'AdminHr', 23, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (19, 'AppointmentLetterGenerated', 'Email', 'Candidate', 13, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (20, 'JoiningBookletAvailable', 'Email', 'Candidate', 14, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (21, 'MedicalLetterAvailable', 'Email', 'Candidate', 15, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (22, 'TargetLetterAvailable', 'Email', 'Candidate', 16, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (23, 'PreBoardingRequested', 'Email', 'Candidate', 17, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (24, 'PreBoardingApproved', 'Email', 'Candidate', 18, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (25, 'PreBoardingCorrectionRequested', 'Email', 'Candidate', 19, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (26, 'PreBoardingSubmitted', 'Email', 'AdminHr', 24, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (27, 'ExportRequestReady', 'Email', 'AdminHr', 25, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (28, 'ExportRequestFailed', 'Email', 'AdminHr', 26, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."EventTemplateMappings" ("EventTemplateMappingId", "RecruitmentEvent", "Channel", "RecipientType", "NotificationTemplateId", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (29, 'PasswordResetRequested', 'Email', 'AdminHr', 4, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);


--
-- Data for Name: NotificationTemplateVersions; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (1, 1, 1, 'Update on your application for {{JobPostingTitle}}', 'Dear {{CandidateName}},

Thank you for applying for {{JobPostingTitle}}. After careful review, we will not be moving forward with your application at this time.

We appreciate your interest and encourage you to apply for future roles that match your profile.

Warm regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (2, 2, 1, 'Your verification code', 'Dear {{CandidateName}},

Your one-time verification code is:

    {{OtpCode}}

This code expires in {{ExpiryMinutes}} minutes. Do not share it with anyone.

If you did not request this, please ignore this email.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (3, 3, 1, 'Password reset code', 'Dear {{CandidateName}},

We received a request to reset your password. Your reset code is:

    {{OtpCode}}

This code expires in {{ExpiryMinutes}} minutes. If you did not request a reset, ignore this email and your password stays unchanged.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (4, 4, 1, 'Confirm your new email address', 'Hello,

Use this code to confirm your new email address:

    {{OtpCode}}

The code expires in {{ExpiryMinutes}} minutes. If you did not request this change, ignore this email.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (5, 5, 1, 'We received your application for {{JobPostingTitle}}', 'Dear {{CandidateName}},

Thank you for applying for {{JobPostingTitle}}. Your application has been received and is currently marked as {{ApplicationStatus}}.

We will review it and keep you updated on next steps. You can track progress in your candidate portal.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (6, 6, 1, 'Update on your application for {{JobPostingTitle}}', 'Dear {{CandidateName}},

The status of your application for {{JobPostingTitle}} has changed from {{FromStatus}} to {{ToStatus}}.

Log in to your candidate portal for details and any required action.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (7, 7, 1, 'Your interview is scheduled', 'Dear {{CandidateName}},

Your interview has been scheduled.

  Date & Time : {{ScheduledStartAt}} to {{ScheduledEndAt}}
  {{LocationLabel}} : {{LocationValue}}

Please be available on time. If you have any conflict, contact us as early as possible.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (8, 8, 1, 'Your interview has been rescheduled', 'Dear {{CandidateName}},

Your interview has been rescheduled to a new time:

  Date & Time : {{ScheduledStartAt}} to {{ScheduledEndAt}}
  {{LocationLabel}} : {{LocationValue}}

Please update your calendar accordingly.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (9, 9, 1, 'Your interview has been cancelled', 'Dear {{CandidateName}},

We regret to inform you that your scheduled interview has been cancelled.

Reason: {{CancellationReason}}

We will contact you if it is to be rescheduled.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (10, 10, 1, 'You are enrolled for {{ExamTitle}}', 'Dear {{CandidateName}},

You have been enrolled for the following exam:

  Exam     : {{ExamTitle}}
  Date     : {{ScheduledStartAt}}
  Duration : {{DurationMinutes}} minutes
  Venue    : {{VenueName}}, {{VenueLocation}}
  Seat No. : {{SeatNumber}}

Carry a valid photo ID. Further instructions will follow with your admit card.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (11, 11, 1, 'Your admit card for {{ExamTitle}}', 'Dear {{CandidateName}},

Your admit card for {{ExamTitle}} is now available.

  Date     : {{ScheduledStartAt}}
  Duration : {{DurationMinutes}} minutes
  Venue    : {{VenueName}}, {{VenueLocation}}
  Seat No. : {{SeatNumber}}

Download your admit card from the candidate portal and bring a printed copy with a valid photo ID.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (12, 12, 1, 'Your offer letter for {{Designation}}', 'Dear {{CandidateName}},

Congratulations! We are pleased to offer you the position of {{Designation}}.

  Annual Salary     : {{OfferedSalary}}
  Joining Date      : {{JoiningDate}}
  Reporting Manager : {{ReportingManager}}
  Valid Until       : {{OfferValidityDate}}

Review and respond to your offer here:
{{PortalLink}}

Please accept or decline before the validity date.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (13, 13, 1, 'Your appointment letter is ready', 'Dear {{CandidateName}},

Your appointment letter for the position of {{Designation}} is now available. Your joining date is {{JoiningDate}}.

Download it here:
{{PortalLink}}

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (14, 14, 1, 'Your joining booklet', 'Dear {{CandidateName}},

Welcome aboard as {{Designation}} (batch {{BatchLabel}}). Your joining date is {{JoiningDate}}.

Your joining booklet with onboarding details is available here:
{{PortalLink}}

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (15, 15, 1, 'Medical examination required', 'Dear {{CandidateName}},

As part of your pre-employment process (Ref: {{CandidateReference}}), a medical examination is required.

  Test Center    : {{MedicalTestCenter}}
  Required Tests : {{RequiredTests}}

Details and your referral letter are here:
{{PortalLink}}

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (16, 16, 1, 'Your performance targets', 'Dear {{CandidateName}},

Your performance targets for the role of {{Designation}} are now available.

KPIs:
{{Kpis}}

Objectives:
{{Objectives}}

View full details here:
{{PortalLink}}

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (17, 17, 1, 'Complete your pre-boarding form', 'Dear {{CandidateName}},

Congratulations on being selected for {{Designation}} (joining {{JoiningDate}}). Before you join, please complete your pre-boarding form.

Fill it here:
{{PortalLink}}

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (18, 18, 1, 'Your pre-boarding submission is approved', 'Dear {{CandidateName}},

Your pre-boarding submission has been reviewed and approved on {{ValidatedAt}}. No further action is needed at this stage.

We look forward to welcoming you.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (19, 19, 1, 'Action needed on your pre-boarding submission', 'Dear {{CandidateName}},

Your pre-boarding submission needs some corrections before it can be approved.

Reviewer comment: {{Comment}}

Please log in and update the requested details.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (20, 20, 1, 'New application for {{JobPostingTitle}}', 'A new candidate has applied.

  Candidate : {{CandidateName}}
  Position  : {{JobPostingTitle}}
  Status    : {{ApplicationStatus}}

Review it in the recruitment dashboard.', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (21, 21, 1, 'Application withdrawn: {{CandidateName}}', 'A candidate has withdrawn their application.

  Candidate : {{CandidateName}}
  Position  : {{JobPostingTitle}}
  Status    : {{FromStatus}} -> {{ToStatus}}', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (22, 22, 1, 'Offer accepted by {{CandidateName}}', 'Good news — an offer has been accepted.

  Candidate   : {{CandidateName}}
  Designation : {{Designation}}
  Accepted At : {{DecisionAt}}', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (23, 23, 1, 'Offer declined by {{CandidateName}}', 'An offer has been declined.

  Candidate   : {{CandidateName}}
  Designation : {{Designation}}
  Declined At : {{DecisionAt}}
  Reason      : {{DeclineReason}}', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (24, 24, 1, 'Pre-boarding submitted by {{CandidateName}}', 'A candidate has submitted their pre-boarding form.

  Candidate    : {{CandidateName}}
  Submitted At : {{SubmittedAt}}

Please review and validate it.', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (25, 25, 1, 'Your export is ready', 'Your requested export has finished generating.

  File : {{FileName}}
  Rows : {{RowCount}}

Download it from the export section of the portal.', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (26, 26, 1, 'Your export failed', 'Your requested export could not be generated.

Reason: {{FailureReason}}

Please try again or contact support if the problem persists.', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (27, 27, 1, 'Complete your payment for {{JobPostingTitle}}', 'Dear {{CandidateName}},

Your application for {{JobPostingTitle}} has been received but is not yet complete. An application fee of {{FeeAmount}} is required to submit it.

Complete your payment here:
{{PaymentLink}}

Your application stays in "{{ApplicationStatus}}" until payment is confirmed. If you have already paid, please ignore this email.

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (28, 28, 1, 'Application awaiting payment: {{CandidateName}}', 'An application is awaiting payment.

  Candidate : {{CandidateName}}
  Position  : {{JobPostingTitle}}
  Fee       : {{FeeAmount}}
  Status    : {{ApplicationStatus}}

It will proceed automatically once the candidate completes payment.', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."NotificationTemplateVersions" ("NotificationTemplateVersionId", "NotificationTemplateId", "VersionNumber", "Subject", "Body", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (29, 29, 1, 'Your result for {{ExamTitle}}', 'Dear {{CandidateName}},

Your result for {{ExamTitle}} has been published.

  Score  : {{Score}} / {{TotalMarks}}
  Pass Mark : {{PassMarks}}
  Result : {{ResultStatus}}

View full details in your portal:
{{PortalLink}}

Regards,
Human Resources', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);


--
-- Data for Name: ReferralSources; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."ReferralSources" ("ReferralSourceId", "Name", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (1, 'LinkedIn', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."ReferralSources" ("ReferralSourceId", "Name", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (2, 'Company Website', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."ReferralSources" ("ReferralSourceId", "Name", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (3, 'Employee Referral', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."ReferralSources" ("ReferralSourceId", "Name", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (4, 'BDJobs', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);


--
-- Data for Name: SpecialCategories; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."SpecialCategories" ("SpecialCategoryId", "Name", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (1, 'Person with Disability', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."SpecialCategories" ("SpecialCategoryId", "Name", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (2, 'Freedom Fighter', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."SpecialCategories" ("SpecialCategoryId", "Name", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (3, 'Female Quota', 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);


--
-- Data for Name: WaiverRules; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public."WaiverRules" ("WaiverRuleId", "Name", "Description", "CandidateTypeFilter", "SpecialCategoryId", "ReferralSourceId", "Priority", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (1, 'Freedom Fighter Fee Waiver', 'Waives full application fee for Freedom Fighter quota candidates', NULL, 2, NULL, 1, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);
INSERT INTO public."WaiverRules" ("WaiverRuleId", "Name", "Description", "CandidateTypeFilter", "SpecialCategoryId", "ReferralSourceId", "Priority", "IsActive", "TenantId", "Remarks", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "DeletedAt", "DeletedBy", "Status") VALUES (2, 'Person with Disability Fee Waiver', 'Waives full application fee for Person with Disability candidates', NULL, 1, NULL, 1, true, 'default_tenant', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0);


--
-- Name: DocumentTemplateVersions_DocumentTemplateVersionId_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

PERFORM pg_catalog.setval('public."DocumentTemplateVersions_DocumentTemplateVersionId_seq"', 7, true);


--
-- Name: DocumentTemplates_DocumentTemplateId_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

PERFORM pg_catalog.setval('public."DocumentTemplates_DocumentTemplateId_seq"', 7, true);


--
-- Name: EventTemplateMappings_EventTemplateMappingId_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

PERFORM pg_catalog.setval('public."EventTemplateMappings_EventTemplateMappingId_seq"', 29, true);


--
-- Name: NotificationTemplateVersions_NotificationTemplateVersionId_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

PERFORM pg_catalog.setval('public."NotificationTemplateVersions_NotificationTemplateVersionId_seq"', 29, true);


--
-- Name: NotificationTemplates_NotificationTemplateId_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

PERFORM pg_catalog.setval('public."NotificationTemplates_NotificationTemplateId_seq"', 29, true);


--
-- Name: ReferralSources_ReferralSourceId_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

PERFORM pg_catalog.setval('public."ReferralSources_ReferralSourceId_seq"', 4, true);


--
-- Name: SpecialCategories_SpecialCategoryId_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

PERFORM pg_catalog.setval('public."SpecialCategories_SpecialCategoryId_seq"', 3, true);


--
-- Name: WaiverRules_WaiverRuleId_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

PERFORM pg_catalog.setval('public."WaiverRules_WaiverRuleId_seq"', 2, true);


--
-- PostgreSQL database dump complete
--


  END IF;
END
$seed$;
""";

        private const string DeleteSql = """
DELETE FROM public."EventTemplateMappings";
DELETE FROM public."NotificationTemplateVersions";
DELETE FROM public."NotificationTemplates";
DELETE FROM public."DocumentTemplateVersions";
DELETE FROM public."DocumentTemplates";
DELETE FROM public."WaiverRules";
DELETE FROM public."ReferralSources";
DELETE FROM public."SpecialCategories";
""";
    }
}
