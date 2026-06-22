import { Routes } from '@angular/router';
import { LayoutComponent } from './core/layout/layout.component';
import { LoginComponent } from './features/auth/login/login.component';
import { authGuard, roleGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: 'careers',
    loadComponent: () =>
      import('./features/public-career/public-career-page.component').then((m) => m.PublicCareerPageComponent),
  },
  {
    path: 'payment/result',
    loadComponent: () =>
      import('./features/payment/payment-result.component').then((m) => m.PaymentResultComponent),
  },
  {
    path: 'payment/success',
    loadComponent: () =>
      import('./features/payment/payment-result.component').then((m) => m.PaymentResultComponent),
  },
  {
    path: 'payment/fail',
    loadComponent: () =>
      import('./features/payment/payment-result.component').then((m) => m.PaymentResultComponent),
  },
  {
    path: 'payment/cancel',
    loadComponent: () =>
      import('./features/payment/payment-result.component').then((m) => m.PaymentResultComponent),
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },
      {
        path: 'job-postings',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/job-postings/job-posting-list.component').then((m) => m.JobPostingListComponent),
      },
      {
        path: 'job-postings/:jobPostingId/pipeline',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/job-postings/pipeline-builder.component').then((m) => m.PipelineBuilderComponent),
      },
      {
        path: 'candidates',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/candidates/candidate-list.component').then((m) => m.CandidateListComponent),
      },
      {
        path: 'requisitions',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/requisitions/requisition-list.component').then((m) => m.RequisitionListComponent),
      },
      {
        path: 'applications',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/applications/application-list.component').then((m) => m.ApplicationListComponent),
      },
      {
        path: 'referrals',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/referrals/referral-list.component').then((m) => m.ReferralListComponent),
      },
      {
        path: 'shortlisting',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/shortlisting/shortlisting.component').then((m) => m.ShortlistingComponent),
      },
      {
        path: 'assessments',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/assessments/assessment-list.component').then((m) => m.AssessmentListComponent),
      },
      {
        path: 'interviews',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/interviews/interview-list.component').then((m) => m.InterviewListComponent),
      },
      {
        path: 'verification',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/verification/verification-list.component').then((m) => m.VerificationListComponent),
      },
      {
        path: 'pre-boarding',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/pre-boarding/pre-boarding-list.component').then((m) => m.PreBoardingListComponent),
      },
      {
        path: 'fitment',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/fitment/fitment-list.component').then((m) => m.FitmentListComponent),
      },
      {
        path: 'onboarding',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/onboarding/onboarding-list.component').then((m) => m.OnboardingListComponent),
      },
      {
        path: 'pipeline/:jobApplicationId',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/pipeline/pipeline-tracker.component').then((m) => m.PipelineTrackerComponent),
      },
      {
        path: 'career-portal',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/career-portal/career-portal.component').then((m) => m.CareerPortalComponent),
      },
      {
        path: 'notifications',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/notifications/notification-list.component').then((m) => m.NotificationListComponent),
      },
      {
        path: 'browse-jobs',
        canActivate: [roleGuard('Candidate')],
        loadComponent: () =>
          import('./features/candidate-jobs/candidate-jobs.component').then((m) => m.CandidateJobsComponent),
      },
      {
        path: 'my-applications',
        canActivate: [roleGuard('Candidate')],
        loadComponent: () =>
          import('./features/candidate-applications/candidate-applications.component').then((m) => m.CandidateApplicationsComponent),
      },
      {
        path: 'my-notifications',
        canActivate: [roleGuard('Candidate')],
        loadComponent: () =>
          import('./features/candidate-notifications/candidate-notifications.component').then((m) => m.CandidateNotificationsComponent),
      },
      {
        path: 'documents',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/documents/document-list.component').then((m) => m.DocumentListComponent),
      },
      {
        path: 'analytics',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/analytics/analytics.component').then((m) => m.AnalyticsComponent),
      },
      {
        path: 'export',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/export/export-list.component').then((m) => m.ExportListComponent),
      },
      {
        path: 'users',
        canActivate: [roleGuard('Admin')],
        loadComponent: () =>
          import('./features/users/user-list.component').then((m) => m.UserListComponent),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/profile/profile.component').then((m) => m.ProfileComponent),
      },
      {
        path: 'integrations',
        canActivate: [roleGuard('Admin')],
        loadComponent: () =>
          import('./features/integrations/integration-list.component').then((m) => m.IntegrationListComponent),
      },
      {
        path: 'profile-field-config',
        canActivate: [roleGuard('Admin')],
        loadComponent: () =>
          import('./features/profile-field-config/profile-field-config.component').then((m) => m.ProfileFieldConfigComponent),
      },
      {
        path: 'application-fees',
        canActivate: [roleGuard('Admin', 'HR')],
        loadComponent: () =>
          import('./features/application-fees/application-fee-config.component').then((m) => m.ApplicationFeeConfigComponent),
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
