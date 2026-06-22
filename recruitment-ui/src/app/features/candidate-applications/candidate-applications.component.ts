import { Component, OnInit, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SlicePipe } from '@angular/common';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-candidate-applications',
  standalone: true,
  imports: [SlicePipe],
  template: `
    <div class="page-header">
      <div>
        <h1 class="page-title">My Applications</h1>
        <p class="page-subtitle">Track your job applications</p>
      </div>
    </div>

    @if (loading()) {
      <div class="loading-state">Loading applications...</div>
    } @else if (applications().length === 0) {
      <div class="empty-state">
        <div class="empty-icon">📄</div>
        <h3>No applications yet</h3>
        <p>Browse open jobs and start applying!</p>
      </div>
    } @else {
      <div class="applications-list">
        @for (app of applications(); track app.jobApplicationId) {
          <div class="app-card">
            <div class="app-card-left">
              <h3 class="app-title">{{ app.jobPostingTitle }}</h3>
              <div class="app-meta">
                <span class="meta-item">Applied: {{ app.appliedDate | slice:0:10 }}</span>
              </div>
            </div>
            <div class="app-card-right">
              <span class="status-badge" [class]="'status-' + getStatusClass(app.applicationStatus)">
                {{ getStatusName(app.applicationStatus) }}
              </span>
            </div>
          </div>
        }
      </div>
    }
  `,
  styles: [`
    .loading-state { text-align: center; padding: 3rem; color: var(--text-secondary, #6b7280); }
    .empty-state { text-align: center; padding: 3rem; }
    .empty-icon { font-size: 3rem; margin-bottom: 1rem; }
    .empty-state h3 { margin: 0 0 0.5rem; }
    .empty-state p { color: var(--text-secondary, #6b7280); }
    .applications-list { display: flex; flex-direction: column; gap: 0.75rem; }
    .app-card { display: flex; justify-content: space-between; align-items: center; background: var(--bg-primary, #fff); border: 1px solid var(--border-color, #e5e7eb); border-radius: 8px; padding: 1rem 1.25rem; }
    .app-title { font-size: 1rem; font-weight: 600; margin: 0 0 0.25rem; color: var(--text-primary, #111827); }
    .app-meta { font-size: 0.813rem; color: var(--text-secondary, #6b7280); }
    .status-badge { padding: 0.25rem 0.75rem; border-radius: 12px; font-size: 0.75rem; font-weight: 600; text-transform: uppercase; }
    .status-applied { background: #dbeafe; color: #1e40af; }
    .status-screening { background: #fef3c7; color: #92400e; }
    .status-shortlisted { background: #d1fae5; color: #065f46; }
    .status-interview { background: #e0e7ff; color: #3730a3; }
    .status-offered { background: #d1fae5; color: #065f46; }
    .status-hired { background: #bbf7d0; color: #14532d; }
    .status-rejected { background: #fee2e2; color: #991b1b; }
    .status-withdrawn { background: #f3f4f6; color: #6b7280; }
  `],
})
export class CandidateApplicationsComponent implements OnInit {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  applications = signal<any[]>([]);
  loading = signal(false);

  ngOnInit(): void {
    this.loading.set(true);
    this.http.get<any>(`${this.baseUrl}/candidate-profile/my-applications`).subscribe({
      next: (res) => {
        this.applications.set(res.content ?? res ?? []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  getStatusName(status: string | number): string {
    const map: Record<string, string> = {
      Applied: 'Applied', Screening: 'Screening', Shortlisted: 'Shortlisted',
      InterviewScheduled: 'Interview Scheduled', Interviewed: 'Interviewed',
      Offered: 'Offered', Hired: 'Hired', Rejected: 'Rejected', Withdrawn: 'Withdrawn',
    };
    return map[status as string] ?? status?.toString() ?? 'Unknown';
  }

  getStatusClass(status: string | number): string {
    const map: Record<string, string> = {
      Applied: 'applied', Screening: 'screening', Shortlisted: 'shortlisted',
      InterviewScheduled: 'interview', Interviewed: 'interview',
      Offered: 'offered', Hired: 'hired', Rejected: 'rejected', Withdrawn: 'withdrawn',
    };
    return map[status as string] ?? 'applied';
  }
}
