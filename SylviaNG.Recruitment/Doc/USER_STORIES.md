# SylviaNG Recruitment System — User Stories

> **Project:** SylviaNG Recruitment & ATS Platform  
> **Version:** 1.0  
> **Date:** June 2026  
> **Prepared by:** SylviaNG Development Team  
> **Status:** In Development — Based on Consolidated Feature Specification

---

## Legend

| Field        | Values                                                           |
| ------------ | ---------------------------------------------------------------- |
| **Story ID** | `US-XXX` — sequential across all epics                           |
| **Epic ID**  | `EP-01` through `EP-17` — maps 1:1 to feature items in spec      |
| **Priority** | `Must Have` · `Should Have` · `Could Have` (MoSCoW)              |
| **Size**     | `XS` · `S` · `M` · `L` · `XL` (T-shirt sizing)                   |
| **Phase**    | `Phase 1` (current release) · `[Phase 2]` (planned next release) |

---

## Actors

| Actor                      | Role in System                                                                          |
| -------------------------- | --------------------------------------------------------------------------------------- |
| **External Candidate**     | Job seeker registering and applying via the career portal from outside the organization |
| **Internal Candidate**     | Existing employee applying to internal vacancies via the employee job board             |
| **HR / Recruiter**         | HR team member who manages end-to-end recruitment, shortlisting, and coordination       |
| **Hiring Manager**         | Line manager or department head who raises requisitions and evaluates candidates        |
| **Interviewer / Panelist** | Subject-matter expert participating in interview panels or evaluation committees        |
| **Admin / Super Admin**    | System administrator responsible for configuration, access control, and integrations    |

---

## Table of Contents

1. [EP-01 — Candidate Profile & CV Management](#ep-01--candidate-profile--cv-management) _(US-001–US-009)_
2. [EP-02 — Job Vacancy Configuration](#ep-02--job-vacancy-configuration) _(US-011–US-016)_
3. [EP-03 — Career Portal & Job Posting](#ep-03--career-portal--job-posting) _(US-018–US-024)_
4. [EP-05 — Application & Candidate Tracking (ATS)](#ep-05--application--candidate-tracking-ats) _(US-033–US-042)_
5. [EP-06 — Shortlisting & Filtering](#ep-06--shortlisting--filtering) _(US-043–US-050)_
6. [EP-07 — Assessment & Evaluation Workflow](#ep-07--assessment--evaluation-workflow) _(US-051–US-062)_
7. [EP-08 — Interview Management](#ep-08--interview-management) _(US-063–US-072)_
8. [EP-09 — Candidate Communication & Notifications](#ep-09--candidate-communication--notifications) _(US-073–US-079)_
9. [EP-10 — Document & Letter Generation](#ep-10--document--letter-generation) _(US-080–US-086)_
10. [EP-11 — Pre-Employment Verification](#ep-11--pre-employment-verification) _(US-087–US-093)_
11. [EP-12 — Hiring & Onboarding Integration](#ep-12--hiring--onboarding-integration) _(US-094–US-099, US-129)_
12. [EP-13 — Data Download & Profile Export](#ep-13--data-download--profile-export) _(US-100–US-104)_
13. [EP-14 — Reports, Analytics & Dashboards](#ep-14--reports-analytics--dashboards) _(US-105–US-110)_
14. [EP-15 — Access Control & User Management](#ep-15--access-control--user-management) _(US-111–US-116)_
15. [EP-16 — System Integrations](#ep-16--system-integrations) _(US-117–US-122)_
16. [EP-17 — Application Fee Management](#ep-17--application-fee-management) _(US-123–US-128)_
17. [Summary](#summary)

---

## EP-01 — Candidate Profile & CV Management

> **Description:** Provides a unified profile for both external and internal candidates. External candidates can self-register, log in, and update personal details at any time without reapplying. Internal candidate profiles are pre-populated from Core HR. AI-based resume parsing auto-populates application fields from PDF/Word CVs.

---

### US-001 — External Candidate Self-Registration

**As an** External Candidate,  
**I want to** register on the recruitment portal by providing my email address and verifying it via OTP,  
**so that** I can create a persistent profile and apply for multiple jobs without re-entering my information each time.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The registration form collects full name, email address, and password as minimum required fields.
- AC2: An OTP is sent to the provided email address upon submission; the account is not activated until OTP is verified.
- AC3: Duplicate email registration is rejected with an informative error message.
- AC4: After successful verification, the candidate is redirected to their profile dashboard.
- AC5: The candidate can resend the OTP if the first one expires (5-minute TTL).

---

### US-002 — Complete Candidate Profile

**As an** External Candidate,  
**I want to** build a comprehensive personal profile including my photo, signature, work experience, education, skills, certifications, family information, and references,  
**so that** HR teams have a complete picture of my background without needing to request additional documents.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: The profile consists of distinct sections: Personal Info, Contact, Education, Work Experience, Skills, Certifications, Documents, and Family Info.
- AC2: Each section can be saved independently; partial completion is allowed.
- AC3: A profile completeness percentage indicator is shown on the dashboard.
- AC4: The candidate can upload a profile photo (JPG/PNG, max 2MB) and a digital signature.
- AC5: Skills can be added via free text or selected from a pre-defined skills library.
- AC6: Education entries require degree, institution, passing year, and result; multiple entries are supported.
- AC7: Work experience entries require company, designation, start/end date, and responsibilities; multiple entries are supported.

---

### US-003 — Update Profile Without Losing Application Data

**As an** External Candidate,  
**I want to** update my profile information at any time,  
**so that** my profile stays current while my previous application data remains unchanged.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Profile edits after application submission do not retroactively change the data snapshot stored at the time of application.
- AC2: The system tags the updated profile to future applications but preserves the original snapshot for past applications.
- AC3: The candidate receives a confirmation message after each save.
- AC4: Fields that are locked after application submission display a visual indicator explaining why they are read-only.

---

### US-004 — Upload and Parse CV Automatically

**As an** External Candidate,  
**I want to** upload my CV in PDF or Word format and have the system automatically extract and populate my profile fields,  
**so that** I can complete my profile quickly without manually entering every detail.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: The system accepts PDF and DOCX formats; maximum file size is 5MB.
- AC2: AI parsing extracts name, contact details, education, work experience, and skills and pre-populates the respective profile sections.
- AC3: The candidate can review and edit all auto-populated fields before saving.
- AC4: If parsing confidence is low for a field, the field is left blank rather than pre-filled with incorrect data.
- AC5: Non-standard CV formats are handled gracefully; a fallback allows manual entry if parsing fails entirely.
- AC6: The original CV file is stored and accessible as a candidate document.

---

### US-005 — Internal Candidate Profile Pre-Population

**As an** Internal Candidate,  
**I want to** have my profile automatically populated from the Core HR system,  
**so that** I do not need to re-enter information that the organization already holds about me.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: When an internal employee logs into the recruitment portal, their name, employee ID, department, designation, and contact details are fetched from Core HR via the integrated API.
- AC2: Pre-populated fields are editable, but any changes are flagged for HR awareness.
- AC3: Internal candidates are required to attach a PDF CV before submitting an application.
- AC4: The system shows a clear distinction between internal and external candidate profiles.

---

### US-006 — Upload Supporting Documents

**As an** External Candidate,  
**I want to** upload supporting documents such as NID, educational certificates, and experience letters to my profile,  
**so that** HR can verify my credentials without requesting them via email.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The system accepts PDF, JPG, PNG, and DOCX formats; maximum file size per document is 10MB.
- AC2: Multiple documents can be uploaded and categorized by type (NID, Education Certificate, Experience Letter, Other).
- AC3: Each uploaded document is accessible to HR via the candidate profile view.
- AC4: The candidate receives a confirmation upon successful upload.
- AC5: Previously uploaded documents can be replaced or deleted by the candidate.

---

### US-007 — View Profile Completeness and Guidance

**As an** External Candidate,  
**I want to** see my profile completeness percentage along with guidance on what sections are incomplete,  
**so that** I know exactly what to fill in to strengthen my application.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A progress bar or percentage indicator is prominently displayed on the candidate dashboard.
- AC2: Incomplete sections are listed with direct links to the relevant form.
- AC3: The completeness score updates in real time as sections are completed.
- AC4: A minimum completeness threshold (configurable by Admin) must be met before an application can be submitted.

---

### US-009 — HR Views Full Candidate Profile

**As an** HR / Recruiter,  
**I want to** view the complete profile of any candidate including all documents, experience, skills, and application history,  
**so that** I can make informed decisions without switching between multiple systems.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The HR candidate profile page consolidates all sections: personal info, education, experience, skills, certifications, documents, and a history of applications.
- AC2: HR can open any uploaded document inline or download it.
- AC3: Profile completeness percentage is shown alongside the profile.
- AC4: Application history shows which jobs the candidate applied to, status, and dates.
- AC5: HR cannot edit the candidate's core profile data but can annotate or tag the profile.

---

## EP-02 — Job Vacancy Configuration

> **Description:** Enables creation of job specifications including responsibilities, descriptions, and number of vacancies. Supports configuration of application fees, circular types (internal/external), and minimum eligibility requirements such as age, education, experience, and candidate district.

---

### US-011 — Create a Job Vacancy

**As an** HR / Recruiter,  
**I want to** create a new job vacancy with a full job specification including title, description, responsibilities, department, employment type, and number of positions,  
**so that** a complete and accurate record of the open role exists in the system before it is advertised.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The vacancy form captures: Job Title, Department, Location, Employment Type (Full-Time / Part-Time / Contract / Internship), Number of Positions, Job Description, and Responsibilities.
- AC2: All required fields are validated before the record can be saved.
- AC3: The vacancy can be saved as a Draft before being published.
- AC4: A unique Job Posting ID is generated upon creation.
- AC5: HR can attach supporting documents to the vacancy (e.g., job profile PDF).

---

### US-012 — Set Salary Range for a Vacancy

**As an** HR / Recruiter,  
**I want to** specify the minimum and maximum salary range for a vacancy,  
**so that** candidates and reviewers have clear compensation expectations.

**Priority:** Should Have  
**Size:** XS

**Acceptance Criteria:**

- AC1: The salary range fields (Min Salary, Max Salary) are available on the vacancy creation form.
- AC2: Both fields are optional; if omitted, salary is displayed as "Negotiable" on the career portal.
- AC3: Min Salary cannot be greater than Max Salary; a validation error is shown if this rule is violated.

---

### US-013 — Configure Minimum Eligibility Requirements

**As an** HR / Recruiter,  
**I want to** set minimum eligibility criteria for a vacancy including age, education level, years of experience, and candidate district,  
**so that** the system can automatically filter out ineligible candidates at the application stage.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Eligibility configuration allows setting Min Age, Max Age, Minimum Education Level, Minimum Experience (years), and Required District.
- AC2: When a candidate applies, the system checks their profile against these criteria and immediately indicates eligibility.
- AC3: Ineligible candidates can still submit an application but are automatically flagged with an eligibility warning visible to HR.
- AC4: Each criterion is optional; a vacancy with no eligibility constraints accepts all applicants.

---

### US-014 — Configure Vacancy as Internal, External, or Both

**As an** HR / Recruiter,  
**I want to** designate a vacancy as internal only, external only, or both internal and external,  
**so that** the job posting reaches only the intended audience.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The circular type field offers three options: Internal Only, External Only, Both.
- AC2: Internal Only vacancies are visible only on the employee job board after login.
- AC3: External Only vacancies are visible only on the public career portal.
- AC4: Both vacancies appear on both boards.
- AC5: Changing the circular type after publication updates visibility immediately.

---

### US-015 — Set Opening and Closing Dates for a Vacancy

**As an** HR / Recruiter,  
**I want to** set a posting date and a closing date for each vacancy,  
**so that** applications are automatically accepted only within the configured date window.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The vacancy form includes Posting Date (start of acceptance) and Closing Date (end of acceptance).
- AC2: The system rejects applications submitted before the posting date or after the closing date.
- AC3: A clearly visible countdown or closing date is displayed on the job listing.
- AC4: HR receives a notification when a vacancy's closing date passes.
- AC5: HR can extend the closing date at any time, subject to Admin authorization if configured.

---

### US-016 — Manage Vacancy Status Lifecycle

**As an** HR / Recruiter,  
**I want to** manage the lifecycle status of a vacancy (Draft → Open → On Hold → Closed → Archived),  
**so that** I have full control over when the role is actively recruiting and can pause or archive it as needed.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Status transitions follow the defined lifecycle: Draft → Open → On Hold ↔ Open → Closed → Archived.
- AC2: Only vacancies in Open status accept new applications.
- AC3: Moving a vacancy to Closed triggers a configurable notification to all applicants who have not yet received a final decision.
- AC4: Archived vacancies are hidden from active lists but remain searchable in history.
- AC5: Status changes are audit-logged with the user and timestamp.

---


## EP-03 — Career Portal & Job Posting

> **Description:** A mobile-responsive public career microsite integrated with the corporate website, supporting video content and testimonials. Includes external job board integrations (BDJobs, LinkedIn), a download/edit option for job postings, and an internal employee job board.

---

### US-018 — Browse Jobs on the Public Career Portal

**As an** External Candidate,  
**I want to** browse all open vacancies on the public career portal without logging in,  
**so that** I can discover relevant job opportunities and decide whether to apply.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The career portal lists all Open vacancies with title, department, location, employment type, and closing date.
- AC2: Candidates can filter vacancies by location, department, employment type, and experience level.
- AC3: A keyword search is available to find jobs matching specific titles or skills.
- AC4: Each listing has a "View Details" page showing the full job description, responsibilities, and requirements.
- AC5: The portal is mobile-responsive and accessible on smartphones and tablets.

---

### US-019 — Apply for a Job via the Career Portal

**As an** External Candidate,  
**I want to** apply for a job directly from the career portal,  
**so that** I can submit my interest in a role without leaving the website.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Clicking "Apply" on a job listing either prompts login/registration or directly opens the application form for logged-in candidates.
- AC2: The application form is pre-populated with the candidate's saved profile data.
- AC3: The candidate can upload a fresh CV or use the one already in their profile.
- AC4: A cover letter field is available but optional.
- AC5: Upon successful submission, a confirmation message and email are sent to the candidate.
- AC6: If an application fee is required, the candidate is redirected to the payment flow before the application is finalized.

---

### US-020 — Manage Career Portal Content

**As an** Admin / Super Admin,  
**I want to** manage the content displayed on the career portal including videos, testimonials, banners, and articles,  
**so that** the portal presents a compelling employer brand that attracts quality candidates.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The admin content management screen supports four content types: Video, Testimonial, Banner, and Article.
- AC2: Each content item can be created, edited, activated, or deactivated without technical knowledge.
- AC3: Videos support URL-based embedding (e.g., YouTube); direct file upload is optional.
- AC4: Testimonials include the employee name, photo, designation, and quote.
- AC5: Content is sorted by a configurable display order and goes live immediately upon activation.

---

### US-021 — Publish a Job to External Job Boards

**As an** HR / Recruiter,  
**I want to** publish a vacancy directly to BDJobs and LinkedIn from the recruitment system,  
**so that** the job reaches the widest possible candidate audience without manual re-posting on external platforms.

**Priority:** Should Have  
**Size:** L

**Acceptance Criteria:**

- AC1: The job posting form includes a Publish Channels section with checkboxes for Career Page, Internal Board, BDJobs, and LinkedIn.
- AC2: Selecting a channel and clicking Publish triggers an API call to the respective platform.
- AC3: The system confirms successful publication with a timestamp and external reference ID.
- AC4: If a publication fails, an error message is shown and HR can retry.
- AC5: BDJobs integration is live in Phase 1; LinkedIn integration is enabled when credentials are configured.

---

### US-022 — Internal Employee Job Board

**As an** Internal Candidate,  
**I want to** browse and apply for internal vacancies through a dedicated employee job board,  
**so that** I can explore career growth opportunities within the organization.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The internal job board is accessible only after employee authentication.
- AC2: It shows only vacancies configured as Internal Only or Both.
- AC3: The interface supports search and filter by department, location, and job type.
- AC4: The employee's existing Core HR profile data is pre-filled in the application form.
- AC5: Applications from the internal board are tracked separately (Source = Internal) in the ATS.

---

### US-023 — Download and Edit a Job Posting

**As an** HR / Recruiter,  
**I want to** download a job posting as a formatted PDF or Word document and edit it offline,  
**so that** I can share it with stakeholders or post it on physical notice boards.

**Priority:** Could Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A "Download" button is available on each job posting detail page (HR view).
- AC2: The download generates a professionally formatted PDF containing the full job specification.
- AC3: An editable DOCX version is also available for offline modification.
- AC4: The downloaded document includes the company logo, job title, requirements, and contact details.

---

### US-024 — Check Eligibility Before Applying

**As an** External Candidate,  
**I want to** see whether I meet the minimum eligibility requirements for a job before I apply,  
**so that** I can save time and apply only to roles I am qualified for.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The job detail page shows a summary of eligibility requirements (age range, education, experience, district).
- AC2: When the candidate is logged in, a real-time eligibility check is performed against their profile.
- AC3: A clear "You are eligible" or "You do not meet the following requirements: …" message is displayed.
- AC4: Ineligible candidates can still apply, but the eligibility warning is shown again at confirmation.

---

## EP-05 — Application & Candidate Tracking (ATS)

> **Description:** End-to-end applicant tracking with separate flows for external and internal candidates. Includes AI-powered resume screening, candidate tagging, talent pool creation, admin-applied applications, duplicate detection, and real-time status tracking.

---

### US-033 — Candidate Submits a Job Application

**As an** External Candidate,  
**I want to** apply for a job by submitting my profile snapshot and CV,  
**so that** my application is formally recorded in the system and visible to HR.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The application form is pre-filled from the candidate's saved profile.
- AC2: A profile snapshot is taken at submission time and stored permanently with the application.
- AC3: The candidate can include a cover letter (optional, max 2000 characters).
- AC4: If a fee is required, the candidate completes payment before the application reaches Submitted status.
- AC5: The candidate receives an email confirmation with the application reference number.
- AC6: Duplicate application prevention: if the candidate has already applied to the same job, submission is blocked.

---

### US-034 — HR Applies on Behalf of a Candidate

**As an** HR / Recruiter,  
**I want to** apply to a job on behalf of a candidate (admin-applied),  
**so that** candidates sourced through agencies or direct outreach are included in the ATS pipeline.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: HR can search for an existing candidate or create a new candidate profile, then submit an application to any open vacancy.
- AC2: The application source is recorded as Admin to distinguish it from self-applied applications.
- AC3: The candidate is notified by email when an application is submitted on their behalf.
- AC4: The application appears in the candidate's My Applications view.

---

### US-035 — View All Applications in the ATS Dashboard

**As an** HR / Recruiter,  
**I want to** view all applications in a consolidated, filterable ATS dashboard,  
**so that** I can efficiently manage the recruitment pipeline across all active vacancies.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The dashboard displays applications with columns: Candidate Name, Job Title, Source, Application Date, and Current Status.
- AC2: Filters are available for Job Posting, Status, Source, Application Date Range, and Candidate Type (External/Internal).
- AC3: Paginated results (configurable page size) with total count displayed.
- AC4: Clicking an application opens the full application detail including the candidate's profile snapshot, CV link, and status history.
- AC5: Bulk status update is available for selected applications (e.g., move 50 applications to Screening at once).

---

### US-036 — Update Application Status Through the Pipeline

**As an** HR / Recruiter,  
**I want to** update the status of an application as it progresses through recruitment stages,  
**so that** the pipeline accurately reflects the current state of every candidate's journey.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Application statuses follow the defined lifecycle: Applied → Screening → Shortlisted → Interview Scheduled → Interviewed → Offered → Hired / Rejected / Withdrawn.
- AC2: Each status change is logged with the actor and timestamp in the application's audit history.
- AC3: Changing status to Rejected or Withdrawn requires a reason to be selected from a configurable list.
- AC4: Candidates are automatically notified upon key status changes (configurable per event in EP-09).
- AC5: HR can add a private note to any status change that is not visible to the candidate.

---

### US-037 — AI-Powered Resume Screening

**As an** HR / Recruiter,  
**I want to** run AI-powered screening on applications for a specific vacancy,  
**so that** the system automatically ranks candidates by relevance, saving hours of manual review.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: The screening engine analyzes each candidate's CV against the job description and produces a relevance score (0–100).
- AC2: Matched keywords, skill tags, and experience band are shown alongside each score.
- AC3: A plain-language explanation of the score is provided for each candidate.
- AC4: HR can trigger a screening run for all applications of a vacancy or re-run it after new applications arrive.
- AC5: Screening results are stored and do not change unless re-triggered.
- AC6: Screening results are visible only to HR, not to candidates.

---

### US-038 — Detect and Resolve Duplicate Applications

**As an** HR / Recruiter,  
**I want to** be alerted when a candidate has applied to the same vacancy through multiple channels (agency and direct),  
**so that** I can identify and merge duplicates to avoid evaluating the same candidate twice.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The system automatically flags applications where the same email, NID, or phone number has already submitted to the same vacancy.
- AC2: Flagged duplicates are shown in a dedicated Duplicates section with side-by-side comparison.
- AC3: HR can mark one as the primary application and dismiss the duplicate.
- AC4: The dismissed application is retained for audit purposes with a "Duplicate Dismissed" status.

---

### US-039 — Manage Talent Pool

**As an** HR / Recruiter,  
**I want to** add promising candidates to named talent pools so I can revisit them for future vacancies,  
**so that** strong but unsuccessful candidates are not lost and can be fast-tracked in future hiring rounds.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: HR can create a named talent pool (e.g., "Senior Engineers – 2026") and add candidates to it from any point in the pipeline.
- AC2: Candidates in a talent pool receive a tag visible in their profile.
- AC3: HR can view all candidates in a talent pool with their latest profile snapshot.
- AC4: When a new vacancy opens, HR can filter talent pool candidates and fast-track them to the shortlist.
- AC5: Candidates can be removed from a talent pool at any time.

---

### US-040 — Candidate Tracks Their Own Application Status

**As an** External Candidate,  
**I want to** view the current status of all my submitted applications in one place,  
**so that** I stay informed about my progress without needing to contact HR.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The My Applications page lists all submitted applications with job title, submission date, and current status.
- AC2: Status is updated in real time as HR progresses the application.
- AC3: The candidate can see scheduled interview dates, assessment links, and document requests from this page.
- AC4: The candidate can withdraw an active application from this view with a confirmation prompt.

---

### US-041 — Tag and Categorize Candidates

**As an** HR / Recruiter,  
**I want to** add custom tags and categories to candidate profiles,  
**so that** I can quickly filter and identify candidates with specific attributes during bulk shortlisting.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: HR can add one or more tags to a candidate's profile (e.g., "Strong Communicator", "Leadership Potential").
- AC2: Tags are free-text with autocomplete suggestions based on previously used tags.
- AC3: The candidate list and ATS dashboard support filtering by tag.
- AC4: Tags are visible to all HR users, not to candidates.

---

### US-042 — View Candidate Pipeline Progress Tracker

**As an** HR / Recruiter,  
**I want to** view a visual pipeline tracker showing a candidate's progress across all configured recruitment stages,  
**so that** I can see at a glance what stage each candidate is in and what actions are outstanding.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The pipeline tracker displays each stage (Screening, Written Test, Interview, etc.) as a horizontal card with status indicators.
- AC2: Each stage card shows status (Pending, In Progress, Completed, Rejected), scheduled date, notes, and meeting link where applicable.
- AC3: HR can start, complete, schedule, reschedule, and add notes to each stage directly from the tracker.
- AC4: Completed stages are visually distinguished from pending stages.
- AC5: The tracker is accessible from both the application detail page and the ATS dashboard.

---

## EP-06 — Shortlisting & Filtering

> **Description:** Automated, customizable multi-layered shortlisting based on configurable screening parameters. Supports Boolean search across all candidate profile fields, bulk selection, job-specific criteria filtering, and admin-driven promotion from shortlist to final selection.

---

### US-043 — Create Automated Shortlist Filter Rules

**As an** HR / Recruiter,  
**I want to** define a set of shortlisting rules (filter criteria) for a vacancy,  
**so that** the system can automatically identify candidates who meet the minimum bar without manual screening of every application.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: HR can create a named shortlist filter associated with a specific job posting.
- AC2: Filter criteria can be based on: Education Level, Minimum Experience Years, Required Skills, Age Range, Location/District, and Minimum Screening Score.
- AC3: Multiple criteria can be combined with AND / OR logic.
- AC4: Filters are saved and can be reused across future vacancies.
- AC5: A preview of the filter shows how many current applications would pass before the filter is applied.

---

### US-044 — Apply Shortlist Filter to a Vacancy

**As an** HR / Recruiter,  
**I want to** apply a saved shortlist filter to all applications of a vacancy with a single action,  
**so that** candidates who meet the criteria are automatically moved to the Shortlisted status.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: HR selects a filter and clicks "Apply to Applications" from the vacancy page.
- AC2: The system evaluates all applications and updates the status of passing candidates to Shortlisted.
- AC3: A summary report is shown after the run: total processed, total shortlisted, total failed.
- AC4: Shortlisted and non-shortlisted candidates are clearly separated in the application list.
- AC5: HR can override the filter result for individual candidates manually.

---

### US-045 — Advanced Boolean Search Across CV Bank

**As an** HR / Recruiter,  
**I want to** search across all candidate profiles and CVs using Boolean operators (AND, OR, NOT) and keyword combinations,  
**so that** I can find specific candidates from the entire talent database for any role.

**Priority:** Should Have  
**Size:** L

**Acceptance Criteria:**

- AC1: The search interface supports full-text Boolean queries against all candidate profile fields and CV content.
- AC2: Operators AND, OR, and NOT are supported and interpreted correctly.
- AC3: Search results display a relevance ranking.
- AC4: HR can filter results further by education, experience range, location, and candidate type.
- AC5: Search results can be selected in bulk and added to a talent pool or shortlist.

---

### US-046 — AI-Powered Auto-Shortlisting

**As an** HR / Recruiter,  
**I want to** use an AI-powered shortlisting engine to rank all applicants against the job description automatically,  
**so that** I can shortlist the top-N candidates without manually reviewing hundreds of CVs.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: The AI shortlisting engine analyzes each application's CV and profile against the job description.
- AC2: Each candidate is given a ranked score with an explanation.
- AC3: HR can configure a cutoff score above which candidates are automatically shortlisted.
- AC4: The AI shortlist is presented as a ranked list with pass/fail indication.
- AC5: HR can adjust the cutoff or override individual decisions after reviewing the list.

---

### US-047 — Bulk Select and Shortlist Candidates

**As an** HR / Recruiter,  
**I want to** select multiple candidates simultaneously and move them to the Shortlisted status in a single action,  
**so that** I can process large volumes of applications efficiently.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Checkboxes allow multi-selection on the application list.
- AC2: A "Move to Shortlist" bulk action is available when one or more applications are selected.
- AC3: The bulk action requires a confirmation prompt showing the count of affected applications.
- AC4: Each affected application is updated individually in the audit trail.
- AC5: Bulk selection works across paginated results (select all on current page or all matching the current filter).

---

### US-048 — Saved Search and Reusable Filter Sets

**As an** HR / Recruiter,  
**I want to** save my frequently used candidate search criteria as a named saved search,  
**so that** I can reapply the same filter combination across different sessions without reconfiguring it.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Any combination of filter criteria applied on the candidate or application list can be saved with a custom name.
- AC2: Saved searches appear in a dropdown for quick access.
- AC3: Saved searches can be edited, renamed, or deleted.
- AC4: Saved searches are personal (per user) by default but can be shared with the HR team if configured.

---

### US-049 — HR Recommends Candidate for Final Selection

**As an** HR / Recruiter,  
**I want to** recommend a shortlisted candidate directly to the final selection pool,  
**so that** exceptional candidates can be fast-tracked to the offer stage without going through every pipeline stage.

**Priority:** Could Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A "Recommend for Final Selection" action is available on the shortlist and pipeline view.
- AC2: The recommendation requires a written justification.
- AC3: The recommendation is visible to the Hiring Manager for approval.
- AC4: The candidate's pipeline tracker is updated to reflect the recommendation.
- AC5: The Hiring Manager can accept or reject the recommendation with comments.

---

### US-050 — Filter Candidates by Job-Specific Criteria

**As an** HR / Recruiter,  
**I want to** filter the applicant list for a vacancy using job-specific criteria such as education, experience, skills, and location,  
**so that** I can quickly drill down to the most relevant candidates for a specific role.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The application list for each vacancy has a dedicated filter panel with criteria specific to that job's requirements.
- AC2: Filter options include Education Level, Experience Range, Skills, Location, Age, and Source.
- AC3: Filters can be combined and applied dynamically; results update in real time.
- AC4: The active filter combination is displayed as removable filter chips.
- AC5: Filters persist for the session duration so HR does not lose them when navigating away.

---

## EP-07 — Assessment & Evaluation Workflow

> **Description:** Configurable multi-stage assessment workflows covering written tests, aptitude tests, psychometric analysis, and multi-round interviews. Includes question group setup, bulk import, auto-assessment, score upload, seat plan generation, admit card distribution, and Exam Hall Management.

---

### US-051 — Create an Assessment Workflow

**As an** HR / Recruiter,  
**I want to** create a named assessment workflow and attach it to a job vacancy,  
**so that** candidates go through a structured, reproducible evaluation process.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: An assessment workflow is created with a name and linked to a job posting or requisition.
- AC2: The workflow consists of one or more ordered assessment stages (e.g., Written Test → Aptitude Test → Psychometric).
- AC3: Each stage has a type, pass mark, and scheduling details.
- AC4: The workflow is activated when the job moves to the Assessment stage.
- AC5: Multiple workflows can be configured and assigned to different job types.

---

### US-052 — Configure Assessment Stages

**As an** HR / Recruiter,  
**I want to** configure individual assessment stages within a workflow including type, duration, and pass criteria,  
**so that** each stage is clearly defined and consistently administered.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Stage types include: Written Test, Aptitude Test, Psychometric Test, Group Discussion, Practical Assessment.
- AC2: Each stage specifies maximum marks, pass marks, and duration in minutes.
- AC3: Stages are ordered within the workflow and the order can be changed via drag-and-drop.
- AC4: A stage can be marked optional or mandatory.
- AC5: Third-party assessment tool integration is configurable per stage (Phase 2).

---

### US-053 — Create and Manage Exam Questions

**As an** HR / Recruiter,  
**I want to** create exam questions of multiple types (MCQ, True/False, Subjective) and organize them into question groups,  
**so that** the assessment content can be structured, reused, and managed at scale.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: Supported question types: MCQ (single/multiple correct), True/False, Short Answer (Subjective).
- AC2: Each question belongs to a Question Group; groups can represent topics or difficulty levels.
- AC3: MCQ questions have configurable options; at least one correct answer must be marked.
- AC4: Questions have fields for marks, difficulty level, and optional explanation.
- AC5: Questions can be searched, filtered by type or group, edited, and deactivated.

---

### US-054 — Bulk Import Exam Questions

**As an** HR / Recruiter,  
**I want to** import a large set of exam questions from an Excel or CSV file,  
**so that** I can populate the question bank quickly without entering questions one by one.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: A downloadable import template (XLSX) is provided with required columns.
- AC2: The import validates each row and reports errors with row numbers.
- AC3: Valid rows are imported; rows with errors are skipped and reported in a summary.
- AC4: Imported questions are placed in the specified question group.
- AC5: A confirmation summary shows the number of questions successfully imported.

---

### US-055 — Schedule an Exam for Shortlisted Candidates

**As an** HR / Recruiter,  
**I want to** schedule an exam for a specific set of shortlisted candidates and configure the exam hall,  
**so that** candidates are formally invited and the logistics are managed in the system.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: HR selects candidates from the shortlist and assigns them to an exam.
- AC2: The exam record captures: title, date, time, duration, total marks, pass marks, exam hall.
- AC3: Exam candidates are enrolled with a unique seat number.
- AC4: An automatic notification (email and SMS) is sent to enrolled candidates with exam details and their admit card.
- AC5: The exam can be in-person (with hall) or online (with auto-assessed questions).

---

### US-056 — Generate Seat Plan for an Exam

**As an** HR / Recruiter,  
**I want to** generate a seating arrangement for an exam and assign candidates to specific seats and halls,  
**so that** invigilators know where each candidate is seated on exam day.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: HR can create one or more exam halls with hall name, location, and total seat capacity.
- AC2: The seat plan is auto-generated by distributing enrolled candidates across available halls.
- AC3: Manual adjustments to seat assignments are supported.
- AC4: The completed seat plan can be downloaded as a printable PDF or Excel sheet.
- AC5: Each candidate's seat number is included in their admit card.

---

### US-057 — Generate and Distribute Admit Cards

**As an** HR / Recruiter,  
**I want to** generate admit cards for all enrolled candidates and distribute them via email and SMS in bulk,  
**so that** candidates have a formal document granting them entry to the exam.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: An admit card is generated for each enrolled candidate containing: name, photo, application number, exam date/time, venue, seat number, and invigilator contact.
- AC2: Admit cards are sent via email with a PDF attachment and via SMS with a download link.
- AC3: Bulk distribution sends to all enrolled candidates for a given exam in a single action.
- AC4: The candidate can download their admit card from the My Applications portal.
- AC5: HR can download admit cards for individual candidates or as a ZIP of all.

---

### US-058 — Candidate Completes an Online Auto-Assessed Exam

**As an** External Candidate,  
**I want to** take an online exam through the recruitment portal where my answers are automatically scored,  
**so that** I can complete assessments remotely and my results are available to HR immediately.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: The exam interface shows questions one at a time or all at once (configurable by HR).
- AC2: A countdown timer is prominently displayed; the exam auto-submits when time expires.
- AC3: MCQ and True/False answers are auto-scored; subjective answers are flagged for manual review.
- AC4: The candidate cannot revisit closed sections or re-submit.
- AC5: Exam results (pass/fail and score) are available to HR immediately after submission.
- AC6: The candidate sees a submission confirmation with a reference number; detailed results are shown only if HR enables it.

---

### US-059 — Upload Manual Assessment Scores

**As an** HR / Recruiter,  
**I want to** upload assessment scores for candidates who completed offline written tests,  
**so that** all evaluation results are consolidated in the system regardless of assessment format.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A score upload form is available per exam, accepting candidate ID and score.
- AC2: Bulk score upload via Excel is supported with a downloadable template.
- AC3: Uploaded scores are validated against the exam's maximum marks.
- AC4: Pass/fail status is automatically calculated based on configured pass marks.
- AC5: Score upload history is audit-logged with the uploader's identity and timestamp.

---

### US-060 — View Assessment Results and Rankings

**As an** HR / Recruiter,  
**I want to** view a ranked list of all candidates who completed an assessment along with their scores and pass/fail status,  
**so that** I can make data-driven decisions about who advances to the next stage.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The results page lists all enrolled candidates sorted by score (descending).
- AC2: Pass/fail status is clearly indicated for each candidate.
- AC3: HR can filter by pass/fail and sort by score, name, or exam date.
- AC4: The results can be exported to Excel for offline review or committee presentation.
- AC5: HR can bulk-move all passing candidates to the next pipeline stage.

---

### US-061 — Integrate with Third-Party Assessment Platforms

**As an** Admin / HR,  
**I want to** configure integration with third-party online assessment and psychometric testing platforms,  
**so that** specialized assessments can be administered externally while results are reflected in the ATS.

**Priority:** Could Have  
**Size:** L  
**Phase:** [Phase 2]

**Acceptance Criteria:**

- AC1: The integration configuration screen allows adding API credentials for third-party assessment providers.
- AC2: When a candidate is assigned to a stage linked to a third-party tool, an assessment invite is sent via the provider's API.
- AC3: Completed results are fetched from the provider and stored against the candidate's application.
- AC4: The result appears in the pipeline tracker alongside all other stage results.
- AC5: If the integration is unavailable, HR is notified and can upload results manually as a fallback.

---

### US-062 — Manage Exam Halls and Invigilators

**As an** HR / Recruiter,  
**I want to** register physical exam halls with their locations, capacities, and assigned invigilators,  
**so that** exam logistics are tracked and invigilators have the information they need on exam day.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Each exam hall record captures: Hall Name, Location/Address, Total Capacity, and assigned Invigilator(s).
- AC2: Halls can be created, updated, and deactivated.
- AC3: A hall is linked to an exam when generating the seat plan.
- AC4: Invigilators assigned to a hall are notified of the exam date and venue.
- AC5: Hall capacity is enforced when assigning candidates; a warning is shown if the hall is near capacity.

---

## EP-08 — Interview Management

> **Description:** End-to-end interview module with scheduling, panel evaluation, custom scorecards, CV viewing, and calendar integration. Supports multiple rounds, bulk scheduling, venue management, scorecard exports, and mobile access.

---

### US-063 — Schedule an Interview for a Candidate

**As an** HR / Recruiter,  
**I want to** schedule an interview for a shortlisted candidate specifying date, time, format, and panel members,  
**so that** both the candidate and panelists are formally invited and calendar events are created.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The interview scheduling form captures: Candidate, Job Posting, Interview Round, Date, Start Time, End Time, Format (In-Person / Virtual), Venue or Meeting Link, and Panel Members.
- AC2: All selected panel members receive a notification with the schedule details.
- AC3: The candidate receives an email and in-app notification with the interview details.
- AC4: A virtual meeting link (e.g., Google Meet) can be generated or pasted in manually.
- AC5: The interview is reflected in the candidate's pipeline tracker as "Interview Scheduled."

---

### US-064 — Reschedule or Cancel an Interview

**As an** HR / Recruiter,  
**I want to** reschedule or cancel an interview and notify all participants,  
**so that** changes are communicated promptly and the pipeline remains accurate.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Reschedule updates the date/time and sends updated notifications to all participants.
- AC2: Cancellation requires a reason and sends a cancellation notice to all participants.
- AC3: Rescheduled interviews are tracked in the audit trail with the reason and new date.
- AC4: The candidate's pipeline tracker reflects the updated scheduled date immediately.

---

### US-065 — Conduct Bulk Interview Scheduling

**As an** HR / Recruiter,  
**I want to** schedule interviews for multiple candidates in one operation,  
**so that** I can efficiently plan interview days with dozens of candidates.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: HR selects multiple shortlisted candidates and assigns them to an interview session.
- AC2: The session has a common date, venue, panel, and round; individual time slots are allocated sequentially.
- AC3: All selected candidates receive individualized notifications with their specific time slot.
- AC4: The interview session overview shows all scheduled candidates with time slots.
- AC5: HR can adjust individual time slots within the session.

---

### US-066 — Panelist Submits Interview Evaluation

**As an** Interviewer / Panelist,  
**I want to** submit my evaluation of a candidate after the interview using a structured scorecard,  
**so that** my assessment is formally recorded and can be compared with other panelists' feedback.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Panelists access their assigned interviews from a dedicated panel dashboard or mobile view.
- AC2: The evaluation form presents the configured scorecard criteria with a rating scale (1–5 or 1–10, configurable).
- AC3: Each criterion allows a score and optional written commentary.
- AC4: An overall recommendation is required: Recommended / Not Recommended / On Hold.
- AC5: Submissions are time-stamped; once submitted, edits require HR approval.
- AC6: The panelist can view the candidate's CV and application snapshot from the evaluation form.

---

### US-067 — Create and Manage Interview Scorecards

**As an** HR / Recruiter,  
**I want to** create reusable interview scorecard templates with weighted evaluation criteria,  
**so that** all panelists evaluate candidates against the same structured framework.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: A scorecard template is created with a name and one or more evaluation criteria.
- AC2: Each criterion has a name, description, and optional weight (percentage).
- AC3: Weights across all criteria in a scorecard must sum to 100%; the system validates this.
- AC4: Templates are reusable across different job postings.
- AC5: HR can clone an existing scorecard template to create a variation.

---

### US-068 — View Aggregated Interview Results

**As an** HR / Recruiter,  
**I want to** view a consolidated summary of all panelists' evaluations for each interviewed candidate,  
**so that** I can make a holistic hiring decision based on collective panel feedback.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The interview results page shows each panelist's scores per criterion and overall recommendation.
- AC2: An aggregate weighted score is calculated and displayed for the candidate.
- AC3: The breakdown by criterion and panelist is shown in a table for side-by-side comparison.
- AC4: HR can see which panelists have not yet submitted their evaluations.
- AC5: The consolidated results can be exported to Excel or PDF.

---

### US-069 — Manage Interview Venues

**As an** HR / Recruiter,  
**I want to** register and manage physical interview venues including room details and booking status,  
**so that** interview logistics are tracked and room conflicts are avoided.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A venue record captures: Venue Name, Location/Address, Room Name, Capacity, and Available Facilities.
- AC2: Venues are available for selection when scheduling in-person interviews.
- AC3: HR can view all interviews scheduled in a given venue on a given date to detect conflicts.
- AC4: Venues can be activated, deactivated, or edited at any time.

---

### US-070 — Create Multi-Round Interview Configuration

**As an** HR / Recruiter,  
**I want to** configure multiple interview rounds for a vacancy (e.g., Technical Round 1, Technical Round 2, HR Round),  
**so that** the evaluation process progresses through defined stages with different panelists and scorecards.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: HR can define named interview rounds for a job posting with a specified sequence.
- AC2: Different scorecards and panel members can be assigned to different rounds.
- AC3: A candidate advances to the next round only when the current round result is marked as Passed.
- AC4: HR can add, remove, or reorder rounds at any time before the interview process begins.
- AC5: The candidate's pipeline tracker shows the current round and all past round results.

---

### US-071 — Export Interview Evaluation Data

**As an** HR / Recruiter,  
**I want to** export all interview evaluations and scores for a vacancy to Excel,  
**so that** I can share results with hiring committees or maintain an offline record.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The export includes: Candidate Name, Round, Panelist, Criteria Scores, Overall Score, Recommendation, and Commentary.
- AC2: Export is available per vacancy or per interview session.
- AC3: The exported file is in XLSX format with each round on a separate sheet.
- AC4: Export is accessible with HR-level or above permissions only.

---

### US-072 — View Interviews on a Mobile Interface

**As an** Interviewer / Panelist,  
**I want to** access my interview schedule and submit evaluations from a mobile device,  
**so that** I can conduct evaluations efficiently whether I am at my desk or in a conference room.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The interview panel dashboard is fully responsive and usable on smartphones.
- AC2: The panelist can view upcoming interviews, candidate CVs, and the scorecard from a mobile browser.
- AC3: Evaluation forms are touch-friendly with large tap targets.
- AC4: Evaluation submission works on mobile with the same validation rules as desktop.

---

## EP-09 — Candidate Communication & Notifications

> **Description:** Multi-channel communication (email, SMS, in-app) automating notifications across all recruitment stages. Includes configurable event setup, bulk notification, and admit card delivery.

---

### US-073 — Configure Notification Templates

**As an** Admin / Super Admin,  
**I want to** create and manage notification templates for each recruitment event across email, SMS, and in-app channels,  
**so that** all candidate communications are professional, consistent, and easy to maintain.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Each template has: Template Name, Channel (Email / SMS / In-App / Push), Subject (for email), Body with placeholder support (e.g., `{{CandidateName}}`, `{{JobTitle}}`).
- AC2: A list of available placeholders is shown while composing the template.
- AC3: Templates can be previewed with sample data substituted for placeholders.
- AC4: Templates are versioned; previous versions are archived and can be restored.
- AC5: Each channel type has channel-specific constraints (e.g., SMS max 160 characters per segment).

---

### US-074 — Map Notification Templates to Recruitment Events

**As an** Admin / Super Admin,  
**I want to** map specific notification templates to recruitment events (e.g., "Application Submitted", "Interview Scheduled", "Offer Issued"),  
**so that** the correct message is sent automatically when each event occurs.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: A list of predefined recruitment events is available: Application Received, Application Shortlisted, Interview Scheduled, Interview Completed, Assessment Invited, Offer Letter Issued, Offer Accepted, Offer Declined, Pre-Boarding Initiated, Joining Confirmed.
- AC2: Each event can be mapped to a template per channel.
- AC3: A channel can be toggled On/Off per event without deleting the mapping.
- AC4: Changes take effect for subsequent event occurrences immediately.

---

### US-075 — Automated Candidate Notification on Status Change

**As an** External Candidate,  
**I want to** receive an automatic email, SMS, or in-app notification whenever my application status changes,  
**so that** I am always informed of my progress without having to check the portal manually.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: A notification is triggered automatically when an application status changes (subject to event-template mapping being configured).
- AC2: Email is the primary channel; SMS and in-app are secondary.
- AC3: Notifications are delivered within 5 minutes of the triggering event.
- AC4: The candidate can view all past notifications in the My Notifications section of the portal.
- AC5: The candidate can configure their notification preferences (opt out of SMS, for example).

---

### US-076 — Send Bulk Notification to a Candidate Group

**As an** HR / Recruiter,  
**I want to** send a bulk notification to a selected group of candidates (e.g., all shortlisted candidates for a vacancy),  
**so that** I can efficiently communicate important updates to many candidates at once.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: HR can select a candidate group from filters (e.g., all applications for a vacancy at a specific status).
- AC2: A compose interface allows writing or selecting a notification template.
- AC3: A preview with the recipient count and a sample substituted message is shown before sending.
- AC4: Bulk sending is confirmed with a progress indicator and completion summary.
- AC5: Each notification is logged in the Notification Log with delivery status.

---

### US-077 — Deliver Admit Cards via Email and SMS

**As an** HR / Recruiter,  
**I want to** automatically send exam admit cards to enrolled candidates via email and SMS upon generation,  
**so that** candidates receive their entry documents without HR needing to manually distribute each one.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Generating or finalizing an exam triggers admit card distribution to all enrolled candidates.
- AC2: Email delivers the admit card as a PDF attachment.
- AC3: SMS delivers a direct download link to the admit card PDF.
- AC4: Distribution status (Sent / Failed) is tracked per candidate.
- AC5: Failed deliveries can be retried individually or in bulk.

---

### US-078 — View Notification Delivery Logs

**As an** HR / Recruiter,  
**I want to** view a log of all notifications sent, including delivery status, channel, and recipient,  
**so that** I can verify communications were delivered and troubleshoot failed sends.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The notification log lists all sent notifications with: Recipient Name, Channel, Event, Sent At, Delivery Status (Sent / Delivered / Failed).
- AC2: Filters are available for date range, channel, event type, and delivery status.
- AC3: Failed notifications can be retried from the log.
- AC4: The log is exportable to Excel.

---

### US-079 — In-App Notification Bell for HR Users

**As an** HR / Recruiter,  
**I want to** see a real-time notification indicator in the application header alerting me to new events requiring my attention,  
**so that** I am immediately aware of approvals, applications, and escalations without refreshing the page.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A notification bell icon in the header shows an unread count badge.
- AC2: Clicking the bell opens a dropdown with the most recent 10 unread notifications.
- AC3: Each notification has a title, timestamp, and a link to the relevant page.
- AC4: Notifications are marked as read when opened; a "Mark all as read" action is available.
- AC5: The unread count updates in real time (polling or WebSocket).

---

## EP-10 — Document & Letter Generation

> **Description:** System-generated, fully customizable document templates for medical letters, appointment letters, target letters, transfer letters, offer summaries, joining info, and post-joining info. Supports digital acceptance tracking and bulk joining booklet downloads.

---

### US-080 — Create and Manage Document Templates

**As an** Admin / Super Admin,  
**I want to** create and manage document templates for all official HR letters using a rich-text editor with placeholder support,  
**so that** consistent, professionally formatted documents can be generated at scale without manual drafting.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: Supported document types: Offer Letter, Appointment Letter, Medical Letter, Target Letter, Transfer Letter, Offer Summary, Joining Information, Post-Joining Information.
- AC2: Templates support placeholders (e.g., `{{CandidateName}}`, `{{DesignationTitle}}`, `{{JoiningDate}}`) that are auto-substituted on generation.
- AC3: A list of available placeholders is displayed next to the editor.
- AC4: Templates support MS Word and PDF output formats.
- AC5: Templates are versioned; the current active version is used for generation.
- AC6: Older versions are archived and can be restored or previewed.

---

### US-081 — Generate an Offer Letter for a Candidate

**As an** HR / Recruiter,  
**I want to** generate a personalized offer letter for a selected candidate using the configured template and their fitment data,  
**so that** the offer is formally communicated in a standardized, ready-to-send document.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: HR selects a candidate from the final selection pool and chooses the Offer Letter template.
- AC2: Candidate-specific and role-specific placeholders are auto-substituted from the candidate's profile and fitment data.
- AC3: HR can preview the generated document before finalizing.
- AC4: The finalized document is saved as a PDF and stored against the candidate's application.
- AC5: The candidate is notified that their offer letter is ready for review.

---

### US-082 — Candidate Accepts or Declines an Offer Digitally

**As an** External Candidate,  
**I want to** view my offer letter online and formally accept or decline it,  
**so that** my decision is recorded digitally without requiring physical paperwork.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The candidate receives a notification with a link to view the offer letter in the portal.
- AC2: The letter is displayed inline as a PDF viewer.
- AC3: Two action buttons are shown: "Accept Offer" and "Decline Offer".
- AC4: Declining requires a mandatory reason.
- AC5: The acceptance/decline decision is time-stamped and locked; it cannot be changed without HR intervention.
- AC6: HR is notified immediately of the candidate's decision.

---

### US-083 — Generate Appointment Letter After Acceptance

**As an** HR / Recruiter,  
**I want to** generate a formal appointment letter after a candidate has accepted their offer,  
**so that** the candidate has a legally valid document confirming their employment terms.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The Generate Appointment Letter action is available only after the offer is accepted.
- AC2: The appointment letter template is pre-filled with candidate, role, salary, and joining date details.
- AC3: HR can review and adjust the content before finalizing.
- AC4: The finalized letter is delivered to the candidate via email and available in the portal.
- AC5: The generation event is logged in the document history.

---

### US-084 — Bulk Generate Joining Booklets

**As an** HR / Recruiter,  
**I want to** generate joining booklets for all candidates in a joining batch in a single action,  
**so that** the onboarding team has all joining documentation ready for the entire cohort at once.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: HR can select a joining batch from the Final Selection Pool.
- AC2: A single action generates joining booklets for all candidates in the batch.
- AC3: Individual booklets are generated using the configured joining booklet template.
- AC4: Bulk download packages all booklets in a single ZIP file.
- AC5: Generation progress is tracked; failures (e.g., missing data) are reported per candidate.

---

### US-085 — Track Document Acceptance Status

**As an** HR / Recruiter,  
**I want to** see the acceptance status of offer letters and appointment letters for all candidates in the pipeline,  
**so that** I can follow up with candidates who have not yet responded.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The generated documents list shows each document's type, recipient, generated date, and acceptance status (Pending / Accepted / Declined).
- AC2: HR can filter by status to quickly find pending responses.
- AC3: A follow-up notification can be triggered for candidates with Pending status.
- AC4: Declining candidates are flagged for HR action (e.g., initiating a replacement hire).

---

### US-086 — Generate Medical and Target Letters

**As an** HR / Recruiter,  
**I want to** generate medical referral letters and target letters for candidates using predefined templates,  
**so that** candidates receive the appropriate documentation for medical tests and performance objectives at the right stage.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Medical letters specify the preferred medical test center and list required tests; the candidate's name and reference are auto-filled.
- AC2: Target letters specify role-specific KPIs and objectives; placeholders are substituted from the role configuration.
- AC3: Both letter types are available for generation from the candidate's application detail page.
- AC4: Generated letters are downloadable as PDF and stored in the document history.
- AC5: The candidate is notified when a new letter is available in their portal.

---

## EP-11 — Pre-Employment Verification

> **Description:** Structured pre-employment background verification covering document verification, employment history, reference checks, medical tests, and due diligence (NID, CIB, CMMS, World Check). _Note: Full implementation is Phase 2; foundational workflows are in Phase 1._

---

### US-087 — Initiate a Background Verification Workflow

**As an** HR / Recruiter,  
**I want to** initiate a background verification workflow for a candidate who has accepted an offer,  
**so that** all necessary pre-employment checks are tracked and completed before the joining date.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: A verification workflow is created for the candidate linked to their job application.
- AC2: The workflow includes a configurable checklist of verification items.
- AC3: The overall workflow status is tracked: Pending / In Progress / Verified / Failed / Discrepancy Found.
- AC4: HR is notified when the overall status changes.
- AC5: The verification outcome is displayed on the candidate's pipeline tracker.

---

### US-088 — Track Individual Verification Items

**As an** HR / Recruiter,  
**I want to** track the status of each individual verification item (NID, Employment History, Reference Check, CIB, Medical, etc.) within a workflow,  
**so that** I know precisely which checks are complete and which are outstanding.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Each item has a type (Document Verification, Employment History, Reference Check, NID, CIB, CMMS, World Check, Medical Test), assigned owner, due date, and status (Not Started / In Progress / Completed / Failed / Discrepancy).
- AC2: Items can be updated individually by the assigned HR user.
- AC3: An item in Discrepancy status triggers an alert for HR review.
- AC4: Completion notes and supporting documents can be attached to each item.
- AC5: The workflow is considered complete only when all mandatory items are marked Completed or a deliberate exception is noted.

---

### US-089 — Schedule and Track Medical Test

**As an** HR / Recruiter,  
**I want to** schedule a medical fitness test for a candidate and track the result,  
**so that** pre-employment medical clearance is formally recorded.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A medical test record is created with: Test Center, Scheduled Date, List of Required Tests, and Status (Scheduled / Completed / Failed).
- AC2: The candidate is notified of the medical test date and location.
- AC3: Results (Fit / Unfit / Conditional) are recorded by HR after the test.
- AC4: An Unfit result triggers an alert for HR to decide whether to proceed, request a second opinion, or withdraw the offer.
- AC5: The medical letter generated in EP-10 pre-fills the test center from this record.

---

### US-090 — Conduct Reference Check

**As an** HR / Recruiter,  
**I want to** record reference check outcomes including contact details, relationship, and feedback for each reference provided by the candidate,  
**so that** professional references are formally verified and documented.

**Priority:** Should Have  
**Size:** S  
**Phase:** [Phase 2]

**Acceptance Criteria:**

- AC1: Reference check items list the candidate's provided references with name, designation, company, and contact details.
- AC2: HR records the outcome of each reference contact: Positive / Neutral / Negative / Unavailable.
- AC3: A free-text commentary field captures the key points from the reference conversation.
- AC4: Two or more reference checks are required before the item is marked Complete.
- AC5: Reference feedback is confidential and accessible only to HR with elevated permissions.

---

### US-091 — Record Document Verification Outcome

**As an** HR / Recruiter,  
**I want to** mark each submitted document (NID, education certificate, experience letter) as Verified or Discrepancy and attach notes,  
**so that** the authenticity of candidate credentials is formally confirmed before hiring.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Document verification items list all documents uploaded by the candidate.
- AC2: HR marks each document as Verified, Discrepancy, or Pending Further Investigation.
- AC3: A discrepancy note is mandatory if the status is set to Discrepancy.
- AC4: Discrepancy items are surfaced in the workflow overview dashboard.
- AC5: Verified documents are locked from further candidate edits.

---

### US-092 — CIB, CMMS, and World Check Verification

**As an** HR / Recruiter,  
**I want to** record the outcome of financial and regulatory due diligence checks (CIB, CMMS, World Check) for a candidate,  
**so that** the organization meets its compliance obligations before making a final hiring decision.

**Priority:** Should Have  
**Size:** M  
**Phase:** [Phase 2]

**Acceptance Criteria:**

- AC1: CIB (Credit Information Bureau), CMMS, and World Check are configured as distinct verification item types.
- AC2: Each item records: Check Type, Date of Check, Result (Clear / Flagged / Inconclusive), and Reference Number.
- AC3: A Flagged result triggers an escalation notification to HR Admin and Legal (configurable).
- AC4: All checks remain confidential and are visible only to HR users with background-check permissions.
- AC5: Check results are included in the verification workflow summary report.

---

### US-093 — Generate Verification Summary Report

**As an** HR / Recruiter,  
**I want to** generate a consolidated pre-employment verification summary report for a candidate,  
**so that** I have a single reference document showing all checks and outcomes to support the hiring committee's final decision.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The report summarizes all verification items with their status, completion date, and key notes.
- AC2: It is generated as a print-ready PDF and stored in the candidate's document record.
- AC3: The report includes an overall clearance status (Cleared / Not Cleared / Pending Items).
- AC4: Generation is available only after all mandatory verification items are in a terminal state.

---

## EP-12 — Hiring & Onboarding Integration

> **Description:** Integrates hiring with Payroll and Core HR for new hire assimilation. Supports single and bulk joining booklets, final selection pool management, onboarding record creation, and office note generation.

---

### US-094 — Manage Final Selection Pool

**As an** HR / Recruiter,  
**I want to** add verified and offer-accepted candidates to a final selection pool with their expected joining date and batch,  
**so that** the onboarding team has a consolidated view of all incoming employees for a joining cycle.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Candidates are added to the pool after their offer is accepted and all verifications are cleared.
- AC2: Each entry captures: Expected Joining Date, Joining Batch, Onboarding Checklist status, and a flag for Has Joined.
- AC3: The pool can be filtered by joining date and batch.
- AC4: When a candidate joins, the "Has Joined" flag is updated and the Actual Joining Date is recorded.
- AC5: The pool view is exportable to Excel.

---

### US-095 — Collect Pre-Boarding Information from Candidate

**As an** External Candidate,  
**I want to** submit my pre-boarding information (nominee details, emergency contacts, insurance preferences) through the portal,  
**so that** my onboarding data is ready in the system before my first day.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: After offer acceptance, the candidate receives a notification to complete the pre-boarding form.
- AC2: The form captures: Personal Details, Nominee(s), Emergency Contact(s), Insurance Details, and Bank Account Info.
- AC3: Multiple nominees can be added with relationship, date of birth, and percentage allocation.
- AC4: The candidate can save as draft and return to complete later.
- AC5: Upon submission, the status changes to Submitted and HR is notified.
- AC6: HR can request corrections; the candidate is notified and the form is unlocked for edits.

---

### US-096 — HR Validates and Locks Pre-Boarding Data

**As an** HR / Recruiter,  
**I want to** review and validate a candidate's submitted pre-boarding data and lock it once confirmed,  
**so that** the data is ready for integration with Core HR and Payroll.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: HR can view the full pre-boarding form alongside the candidate's profile.
- AC2: HR can mark the form as Validated or Request Corrections with specific comments.
- AC3: Once validated, the form can be locked to prevent further changes.
- AC4: Locked pre-boarding data is used as the source for Core HR onboarding record creation.
- AC5: Correction requests unlock the form for the candidate and remove the Validated status.

---

### US-097 — Configure Fitment Data for Salary and Grade

**As an** HR / Recruiter,  
**I want to** configure a candidate's proposed grade, designation, and salary structure (fitment data) before generating the offer letter,  
**so that** the compensation details are accurate and aligned with Payroll.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The fitment form captures: Proposed Grade, Proposed Designation, Location, and Salary Structure (basic, allowances, deductions).
- AC2: Salary structure can be auto-fetched from the Payroll system or entered manually.
- AC3: When fetched from Payroll, the source is flagged and the data is read-only unless overridden.
- AC4: Manual entries are flagged for compensation team review.
- AC5: Fitment data is stored against the application and feeds into offer letter generation.

---

### US-098 — Create Onboarding Record and Sync with Core HR

**As an** HR / Recruiter,  
**I want to** create an onboarding record for a new hire and trigger the sync with the Core HR system,  
**so that** a new employee record is automatically created in Core HR on the day of joining.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: An onboarding record is created from the final selection pool entry after all pre-boarding data is validated.
- AC2: A "Send to Core HR (Pre-Hire)" action transmits essential data (name, NID, role, joining date) to Core HR via the configured integration.
- AC3: Core HR returns an Employee ID which is stored in the onboarding record.
- AC4: A "Send to Core HR (Post-Hire)" action completes the full employee record after the actual joining date.
- AC5: Both sync actions are retryable; failure details and retry count are logged.
- AC6: HR is notified of success or failure for each sync action.

---

### US-099 — Sync Salary Structure with Payroll

**As an** HR / Recruiter,  
**I want to** push the finalized salary structure to the Payroll system after a new hire joins,  
**so that** the employee's first payroll is processed correctly without manual data re-entry.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The payroll sync action is available after the Post-Hire Core HR sync succeeds.
- AC2: The salary structure from fitment data is transmitted to the Payroll system API.
- AC3: A Payroll Reference ID is returned and stored in the onboarding record.
- AC4: Payroll sync failures are logged with error details and can be retried.
- AC5: HR is notified of sync success or failure.

---

### US-129 — Generate Office Note for Onboarding Enclosures

**As an** HR / Recruiter,  
**I want to** generate an office note listing the enclosures accompanying a new hire's onboarding file (offer letter, appointment letter, verification summary, joining booklet),  
**so that** the personnel file has a formal internal record referencing every enclosed document without manually compiling the list.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The office note is generated from the candidate's onboarding record once the joining booklet and verification summary are available.
- AC2: The note auto-lists all generated enclosures (offer letter, appointment letter, verification summary, joining booklet) with document name and generation date.
- AC3: HR can add free-text remarks to the office note before finalizing.
- AC4: The finalized office note is saved as a PDF and stored in the candidate's document history alongside the other onboarding documents.
- AC5: The office note can be generated individually or in bulk for an entire joining batch.

---

## EP-13 — Data Download & Profile Export

> **Description:** Downloadable Excel/CSV reports for shortlisted candidates in single-line-per-candidate format. Supports bulk CV PDF downloads, admit card downloads, and system-generated profile exports for offline use.

---

### US-100 — Export Candidate List to Excel/CSV

**As an** HR / Recruiter,  
**I want to** export a list of all or filtered candidates to Excel or CSV format,  
**so that** I can share candidate data with stakeholders or use it for offline analysis.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Any filtered or selected candidate list can be exported from the ATS dashboard.
- AC2: The export includes: Candidate Name, Email, Phone, Application Date, Job Title, Status, Education, Experience, and Screening Score.
- AC3: Each candidate occupies exactly one row (single-line-per-candidate format).
- AC4: Both XLSX and CSV formats are available.
- AC5: Large exports (>1000 rows) are processed asynchronously; HR is notified when the file is ready for download.

---

### US-101 — Bulk Download Candidate CVs

**As an** HR / Recruiter,  
**I want to** bulk download the CVs of all shortlisted candidates for a vacancy as a single ZIP file,  
**so that** I can distribute CVs to a hiring committee without downloading each one individually.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: A "Bulk Download CVs" action is available on the application list when candidates are selected.
- AC2: All selected candidates' latest CV files are bundled into a ZIP archive.
- AC3: Each file in the ZIP is named with the format: `CandidateName_ApplicationID.pdf`.
- AC4: The download is processed asynchronously for large batches; HR is notified when the ZIP is ready.
- AC5: Only CVs accessible to the requester's permission level are included.

---

### US-102 — Bulk Download Admit Cards

**As an** HR / Recruiter,  
**I want to** bulk download all admit cards for a scheduled exam as a single ZIP file,  
**so that** physical distribution can be arranged for in-person exams.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A "Download All Admit Cards" button is available on the exam detail page.
- AC2: All generated admit cards are bundled into a ZIP with one PDF per candidate.
- AC3: File naming follows: `AdmitCard_ExamName_CandidateName.pdf`.
- AC4: Admit cards for candidates whose delivery failed are included in the ZIP as a fallback.

---

### US-103 — Download System-Generated Candidate Profile

**As an** HR / Recruiter,  
**I want to** download a formatted, system-generated profile summary for any candidate,  
**so that** I can share a standardized profile with interview panels without sending the raw CV.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A "Download Profile" button is available on every candidate profile page.
- AC2: The generated PDF includes: photo, personal details, education summary, experience summary, skills, certifications, and screening score.
- AC3: The layout is clean and uses the organization's branding.
- AC4: The download is available immediately without any asynchronous processing.

---

### US-104 — Submit and Track Data Export Requests

**As an** HR / Recruiter,  
**I want to** submit a large data export request and track when it is ready to download,  
**so that** I can request large reports without the system timing out.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Large exports (above a configurable threshold) are submitted as export requests rather than immediate downloads.
- AC2: The export request is queued and processed in the background.
- AC3: HR receives an in-app and email notification when the export file is ready.
- AC4: The export is downloadable from an Export Requests list for a configurable retention period (e.g., 7 days).
- AC5: Export request history shows status, file type, row count, and creation date.

---

## EP-14 — Reports, Analytics & Dashboards

> **Description:** Real-time dashboards with customizable funnel/pipeline reports and data export. Includes a centralized Job Application Tracker, candidate dashboards, interview analytics, and configurable settings dashboards.

---

### US-105 — Recruitment Overview Dashboard

**As an** HR / Recruiter,  
**I want to** see a real-time recruitment overview dashboard showing key metrics across all active vacancies,  
**so that** I have an at-a-glance picture of the overall hiring pipeline without pulling individual reports.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: The dashboard shows: Total Open Vacancies, Total Applications (by status), Upcoming Interviews, Pending Approvals, and Offers Pending Acceptance.
- AC2: Metrics are presented as cards with trend indicators (change vs. last week/month).
- AC3: Clicking a metric card navigates to the relevant filtered list.
- AC4: The dashboard refreshes automatically every 5 minutes or on manual reload.
- AC5: Admin can configure which widgets are shown on the dashboard (EP-15).

---

### US-106 — Recruitment Funnel Visualization

**As an** HR / Recruiter,  
**I want to** view a funnel chart showing how many candidates progressed through each pipeline stage for a specific vacancy or time period,  
**so that** I can identify where candidates are dropping off and optimize the hiring process.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The funnel chart shows candidate counts at each stage: Applied → Screened → Shortlisted → Assessed → Interviewed → Offered → Hired.
- AC2: Filters are available for Job Posting, Department, Date Range, and Candidate Type.
- AC3: The funnel shows both absolute numbers and conversion rates between stages.
- AC4: Hovering over a stage reveals a breakdown of pass and drop-off reasons if recorded.
- AC5: The chart is exportable as PNG and the underlying data as CSV.

---

### US-107 — Time-to-Hire Analytics

**As an** HR / Recruiter,  
**I want to** view time-to-hire metrics showing how long each vacancy took from posting to offer acceptance,  
**so that** I can benchmark performance and identify bottlenecks in the hiring timeline.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Time-to-hire is calculated per vacancy as days from Job Posted to Offer Accepted.
- AC2: Average, minimum, and maximum values are shown across filtered vacancies.
- AC3: A breakdown by stage shows average time spent in each pipeline step.
- AC4: The view can be filtered by department, hiring manager, and date range.
- AC5: Data is exportable for trend analysis.

---

### US-108 — Candidate Source Analytics

**As an** HR / Recruiter,  
**I want to** see a breakdown of candidate sources (Career Portal, BDJobs, LinkedIn, Internal, Employee Referral, Agency),  
**so that** I can evaluate the ROI of each sourcing channel and optimize where to invest.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: A pie or bar chart shows the distribution of applications by source.
- AC2: The chart can be filtered by vacancy, date range, and employment type.
- AC3: Each source segment shows: total applications, shortlisted count, hired count, and conversion rate.
- AC4: Data is exportable to Excel.

---

### US-109 — Job Application Tracker Report

**As an** HR / Recruiter,  
**I want to** view a centralized job application tracker showing all applications across all vacancies in one tabular report,  
**so that** I can monitor pipeline health and take action on stalled applications.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The tracker table shows: Vacancy, Candidate Name, Stage, Status, Last Updated, Days in Current Stage, and Assigned HR.
- AC2: Rows with "Days in Current Stage" exceeding a configurable threshold are highlighted.
- AC3: The table is sortable and filterable by all columns.
- AC4: HR can trigger a status update or note directly from the tracker row without navigating away.
- AC5: The tracker is exportable to Excel.

---

### US-110 — Interview Analytics Export

**As an** HR / Recruiter,  
**I want to** view and export interview performance analytics including panelist participation rates, average evaluation scores, and recommendation distributions,  
**so that** interview quality and panel effectiveness can be reviewed and improved.

**Priority:** Could Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The analytics view shows per-panelist stats: number of interviews conducted, average score given, and recommendation distribution (Recommended / Not Recommended / On Hold).
- AC2: Score distributions are shown as a histogram.
- AC3: Filtered by vacancy, department, and date range.
- AC4: Exportable to Excel for committee review.

---

## EP-15 — Access Control & User Management

> **Description:** Role-based access control (RBAC) with Admin, HR, Hiring Manager, Interviewer, and Candidate views. Supports user creation, role assignment, permission management, and candidate profile field configuration.

---

### US-111 — Create and Manage HR User Accounts

**As an** Admin,  
**I want to** create, edit, and deactivate user accounts for HR staff and assign them to roles,  
**so that** the right team members have access to the recruitment system with appropriate permissions.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Admin creates a user by providing: Full Name, Email, Employee ID, Department, and Role(s).
- AC2: The user receives an email invitation to set their password via Keycloak.
- AC3: A user can be assigned multiple roles.
- AC4: Deactivating a user revokes their session and prevents future login.
- AC5: Deactivated users cannot be reassigned active tasks; open tasks are flagged for reassignment.

---

### US-112 — Create and Manage Roles and Permissions

**As an** Admin / Super Admin,  
**I want to** create custom roles and define their permissions at the resource level (View, Create, Edit, Delete, Approve),  
**so that** access is precisely controlled and follows the principle of least privilege.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: Predefined roles (Admin, HR, Hiring Manager, Interviewer, Candidate) cannot be deleted but can be cloned.
- AC2: Custom roles can be created with any combination of resource-level permissions.
- AC3: Permissions are organized by module: Requisitions, Job Postings, Applications, Interviews, Assessments, Verifications, Reports, Admin.
- AC4: Role changes take effect on the user's next login.
- AC5: The permission matrix is displayed as a grid for easy auditing.

---

### US-113 — Role-Based Access for Hiring Managers

**As a** Hiring Manager,  
**I want to** see only the requisitions, applications, and candidates relevant to my department and the roles I am assigned to evaluate,  
**so that** I am not overwhelmed with organization-wide data and confidentiality is maintained.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: A Hiring Manager can only view and act on requisitions they created or are assigned to.
- AC2: Application lists are scoped to job postings linked to their requisitions.
- AC3: Candidate details outside their scope are not accessible.
- AC4: Dashboard widgets show metrics filtered to the manager's scope.

---

### US-114 — Candidate Role — Self-Service Access Only

**As an** External Candidate,  
**I want to** access only my own profile, applications, notifications, and pre-boarding data,  
**so that** I cannot see other candidates' information or HR-internal data.

**Priority:** Must Have  
**Size:** S

**Acceptance Criteria:**

- AC1: Candidates cannot access any HR-internal pages (ATS dashboard, candidate list, requisitions, etc.).
- AC2: Candidates can only read and edit their own profile and applications.
- AC3: Any attempt to access a restricted URL redirects to the candidate portal home.
- AC4: API calls made with a candidate token are rejected with a 403 Forbidden response for HR-only endpoints.

---

### US-115 — Admin Impersonation with Full Audit Logging

**As an** Admin / Super Admin,  
**I want to** impersonate another user's account for support purposes with full session logging,  
**so that** I can troubleshoot issues on behalf of users while maintaining a complete audit trail for compliance.

**Priority:** Could Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Admin can initiate an impersonation session for any user via a dedicated action in User Management.
- AC2: While impersonating, a visible banner indicates the impersonation state.
- AC3: Every action taken during the impersonation session is logged in the Impersonation Log with: Admin ID, Target User ID, Session Start, Session End, and Actions Performed.
- AC4: Impersonation sessions automatically expire after 30 minutes.
- AC5: Impersonation logs are read-only and accessible only to Super Admin.

---

### US-116 — Configure Mandatory Candidate Profile Fields

**As an** Admin / Super Admin,  
**I want to** configure which candidate profile fields are mandatory, optional, or hidden globally or per job type,  
**so that** candidates provide exactly the information our organization requires.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The configuration screen shows all candidate profile fields with Mandatory / Optional / Hidden toggles.
- AC2: Changes can be applied globally or scoped to a specific job category.
- AC3: Mandatory fields are marked with a visual indicator in the candidate profile form.
- AC4: A minimum core set of fields (Full Name, Email, CV) cannot be changed to Hidden.
- AC5: Configuration changes are effective for new profile submissions after the change is saved.

---

## EP-16 — System Integrations

> **Description:** End-to-end integration with SylviaHCM (Core HR), Payroll, BDJobs, LinkedIn, HR mobile apps, SSL Commerz, Keycloak, Kafka, and third-party assessment/video interview platforms.

---

### US-117 — Configure Core HR Integration

**As an** Admin / Super Admin,  
**I want to** configure the connection settings for the Core HR (SylviaHCM) integration,  
**so that** employee data, org structure, and onboarding records can be exchanged between the recruitment system and Core HR.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The integration configuration screen captures: API Base URL, Authentication Method (gRPC / REST), and connection credentials.
- AC2: A "Test Connection" button verifies the configuration before saving.
- AC3: The integration supports bi-directional data flow: fetch employee data (for internal candidates) and push new hire records.
- AC4: Integration logs record each exchange: timestamp, direction, payload summary, and response status.
- AC5: Failed integration calls trigger an alert to the Admin.

---

### US-118 — Configure Payroll Integration

**As an** Admin / Super Admin,  
**I want to** configure the Payroll system integration for salary structure exchange,  
**so that** fitment data is fetched from Payroll and new hire salary data is pushed back after onboarding.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Payroll integration configuration stores API endpoint, authentication, and data mapping rules.
- AC2: HR can trigger a "Fetch Fitment from Payroll" action for a candidate from the fitment screen.
- AC3: After joining, HR can trigger a "Push Salary to Payroll" action from the onboarding record.
- AC4: All API calls are logged with status and timestamp.
- AC5: Data mapping configuration is editable without code changes.

---

### US-119 — Configure External Job Board Integrations (BDJobs / LinkedIn)

**As an** Admin / Super Admin,  
**I want to** configure API credentials for BDJobs and LinkedIn job posting integrations,  
**so that** HR can publish vacancies to external job boards directly from the recruitment system.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Each integration (BDJobs, LinkedIn) has its own configuration record with API key/secret and endpoint settings.
- AC2: A connection test is available for each integration.
- AC3: Active integrations appear as channel options when creating or editing a job posting.
- AC4: Inactive or unconfigured integrations are disabled in the job posting channel selector.
- AC5: Integration logs capture each publication attempt with status and external reference ID.

---

### US-120 — Configure SSL Commerz Payment Integration

**As an** Admin / Super Admin,  
**I want to** configure the SSL Commerz payment gateway credentials,  
**so that** application fee payments are processed securely through the integrated gateway.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Configuration captures: Store ID, Store Password, and environment (Sandbox / Production).
- AC2: The system switches between sandbox and production modes without code changes.
- AC3: IPN (Instant Payment Notification) callback URL is configurable.
- AC4: A test transaction can be initiated from the configuration screen in sandbox mode.
- AC5: Payment gateway configuration changes are audit-logged.

---

### US-121 — View Integration Logs and Diagnose Failures

**As an** Admin / Super Admin,  
**I want to** view detailed integration execution logs and diagnose API call failures,  
**so that** integration issues are identified and resolved quickly without involving the development team.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The integration log list shows: Integration Name, Operation, Timestamp, Status (Success / Failed), HTTP Status Code, and Duration.
- AC2: Clicking a log entry shows the full request payload and response body.
- AC3: Filters are available for integration type, status, and date range.
- AC4: Failed calls can be retried directly from the log entry where supported.
- AC5: Logs are retained for a configurable period (default: 90 days).

---

### US-122 — Configure Kafka Event Streaming

**As an** Admin / Super Admin,  
**I want to** configure Kafka broker settings for event-driven integration with downstream systems,  
**so that** events such as new hire creation are reliably published to consuming services.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Kafka configuration stores: Bootstrap Servers, Topic Names, Consumer Group ID, and Security settings.
- AC2: A connection health check is available from the configuration screen.
- AC3: Published events are logged with topic, partition, offset, and timestamp.
- AC4: Dead-letter events (failed publishes) are captured and retryable.
- AC5: Kafka configuration changes require a service restart confirmation prompt.

---

## EP-17 — Application Fee Management

> **Description:** Configures and manages application fees per vacancy, tracks payment status per candidate, reconciles collected payments, and links successful payment to application submission. Primary payment channel: bKash and SSL Commerz.

---

### US-123 — Configure Application Fee for a Vacancy

**As an** Admin / Super Admin,  
**I want to** configure an application fee for a specific vacancy including amount, payment methods, and waiver rules,  
**so that** the fee requirement is clear and consistently enforced for all applicants.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Fee configuration per vacancy captures: Fee Amount, Currency, and accepted Payment Methods (bKash, SSL Commerz card, etc.).
- AC2: A "No Fee" option is available; vacancies with no fee accept applications without payment.
- AC3: Waiver rules can be configured: e.g., waive fee for internal candidates or candidates from specific categories.
- AC4: Fee configuration is displayed on the job listing for candidates.
- AC5: Only Admin can change fee configuration after a vacancy is published; changes apply to new applications only.

---

### US-124 — Candidate Pays Application Fee

**As an** External Candidate,  
**I want to** pay the required application fee through a secure payment gateway before my application is submitted,  
**so that** my application is formally accepted and I receive a payment receipt.

**Priority:** Must Have  
**Size:** L

**Acceptance Criteria:**

- AC1: After completing the application form, the candidate is redirected to the payment step if a fee is configured.
- AC2: The payment page shows the job title, fee amount, and available payment methods.
- AC3: After successful payment via SSL Commerz, the candidate is redirected to a success page and the application status changes to Submitted.
- AC4: A payment receipt with transaction ID and amount is emailed to the candidate.
- AC5: If payment fails or is cancelled, the candidate is returned to the payment page and the application remains in Draft.

---

### US-125 — View Payment Transaction History

**As an** HR / Recruiter,  
**I want to** view a list of all payment transactions for a vacancy or across all vacancies,  
**so that** I can reconcile collected fees and identify any pending or failed payments.

**Priority:** Should Have  
**Size:** S

**Acceptance Criteria:**

- AC1: The payment list shows: Candidate Name, Job Title, Transaction ID, Payment Method, Amount, Status (Pending / Success / Failed / Refunded), and Date.
- AC2: Filters are available for vacancy, date range, and payment status.
- AC3: The total collected amount is shown as a summary.
- AC4: The list is exportable to Excel for accounting purposes.

---

### US-126 — Verify Payment Status via IPN Callback

**As an** Admin / Super Admin,  
**I want to** ensure that payment confirmations from SSL Commerz are processed via IPN callbacks and cross-verified,  
**so that** only genuinely successful payments unlock application submission.

**Priority:** Must Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The system exposes an IPN endpoint that SSL Commerz calls upon payment completion.
- AC2: The IPN handler verifies the transaction against SSL Commerz's validation API.
- AC3: Only transactions with a VALID status from the verification call update the application to Submitted.
- AC4: IPN failures (network or validation mismatch) are logged and trigger an Admin alert.
- AC5: A manual "Verify Payment" action is available for transactions that may have been missed by IPN.

---

### US-127 — Configure Fee Waiver and Exemption Rules

**As an** Admin / Super Admin,  
**I want to** define rules that exempt certain candidate categories from paying the application fee,  
**so that** fairness policies (e.g., waiving fees for persons with disabilities) are enforced automatically.

**Priority:** Could Have  
**Size:** M

**Acceptance Criteria:**

- AC1: Waiver rules can be based on: Candidate Type (Internal), Special Category (configurable), or Referral Source.
- AC2: When a waiver rule applies to a candidate, the payment step is skipped and the application is submitted directly.
- AC3: The system records that a waiver was applied and the applicable rule.
- AC4: Waived applications appear in the payment report with a Waived status.
- AC5: Waiver rules are audited; only Admin can create or modify them.

---

### US-128 — Reconcile Application Fee Collections

**As an** Admin / Super Admin,  
**I want to** generate a fee reconciliation report for a given period showing total collected, total failed, and total waived,  
**so that** Finance has accurate data for accounting and the organization can audit payment integrity.

**Priority:** Should Have  
**Size:** M

**Acceptance Criteria:**

- AC1: The reconciliation report covers a configurable date range.
- AC2: It shows: Total Applications with Fee, Total Paid, Total Failed, Total Waived, Net Collected Amount.
- AC3: A line-item breakdown lists each transaction with candidate name, vacancy, amount, status, and transaction ID.
- AC4: The report is exportable to Excel and PDF.
- AC5: The report can be scoped to a specific vacancy, department, or organization-wide.

---

## Summary

### Stories by Epic

| Epic      | Title                                   | Story Count |
| --------- | --------------------------------------- | ----------- |
| EP-01     | Candidate Profile & CV Management       | 8           |
| EP-02     | Job Vacancy Configuration               | 6           |
| EP-03     | Career Portal & Job Posting             | 7           |
| EP-05     | Application & Candidate Tracking (ATS)  | 10          |
| EP-06     | Shortlisting & Filtering                | 8           |
| EP-07     | Assessment & Evaluation Workflow        | 12          |
| EP-08     | Interview Management                    | 10          |
| EP-09     | Candidate Communication & Notifications | 7           |
| EP-10     | Document & Letter Generation            | 7           |
| EP-11     | Pre-Employment Verification             | 7           |
| EP-12     | Hiring & Onboarding Integration         | 7           |
| EP-13     | Data Download & Profile Export          | 5           |
| EP-14     | Reports, Analytics & Dashboards         | 6           |
| EP-15     | Access Control & User Management        | 6           |
| EP-16     | System Integrations                     | 6           |
| EP-17     | Application Fee Management              | 6           |
| **Total** |                                         | **118**     |

### Stories by Priority

| Priority    | Count | %   |
| ----------- | ----- | --- |
| Must Have   | 76    | 64% |
| Should Have | 32    | 27% |
| Could Have  | 10    | 8%  |

### Stories by Actor

| Actor                  | Stories Involve                                                                                                                                                                                    |
| ---------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| External Candidate     | US-001–007, US-009, US-019, US-024, US-033, US-040, US-058, US-075, US-082, US-095, US-114, US-124                                                                                                 |
| Internal Candidate     | US-005, US-022, US-040                                                                                                                                                                             |
| HR / Recruiter         | US-009, US-011–016, US-021, US-023, US-034–039, US-041–050, US-053–057, US-059–060, US-063–068, US-071, US-077–079, US-081, US-083–086, US-087–091, US-093–094, US-096–099, US-100–104, US-105–110, US-129 |
| Hiring Manager         | US-113                                                                                                                                                                                             |
| Interviewer / Panelist | US-066, US-072                                                                                                                                                                                     |
| Admin / Super Admin    | US-020, US-028, US-061, US-073, US-074, US-080, US-111, US-112, US-115–116, US-117–122, US-123, US-126–128                                                                                         |

### Phase Summary

| Phase             | Stories                |
| ----------------- | ---------------------- |
| Phase 1 (Current) | 115 stories            |
| Phase 2 (Planned) | US-061, US-090, US-092 |

---

_Document end — SylviaNG Recruitment User Stories v1.0_
