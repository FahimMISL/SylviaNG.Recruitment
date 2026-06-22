import { Component, OnInit, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [DatePipe, RouterLink],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Analytics</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Analytics & Reports</h1>
        <p class="page-subtitle">Recruitment metrics and pipeline overview</p>
      </div>
    </div>

    @if (loading()) {
      <div class="loading-state">Loading analytics...</div>
    } @else if (data()) {
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-value">{{ data()!.summary.totalCandidates }}</div>
          <div class="stat-label">Total Candidates</div>
        </div>
        <div class="stat-card">
          <div class="stat-value">{{ data()!.summary.totalApplications }}</div>
          <div class="stat-label">Applications</div>
        </div>
        <div class="stat-card">
          <div class="stat-value">{{ data()!.summary.totalJobPostings }}</div>
          <div class="stat-label">Job Postings</div>
        </div>
        <div class="stat-card">
          <div class="stat-value">{{ data()!.summary.totalInterviews }}</div>
          <div class="stat-label">Interviews</div>
        </div>
        <div class="stat-card">
          <div class="stat-value">{{ data()!.summary.totalReferrals }}</div>
          <div class="stat-label">Referrals</div>
        </div>
        <div class="stat-card accent">
          <div class="stat-value">{{ data()!.summary.totalOnboarded }}</div>
          <div class="stat-label">Onboarded / Hired</div>
        </div>
      </div>

      <div class="panels">
        <div class="panel">
          <h3 class="panel-title">Application Status Breakdown</h3>
          @if (data()!.statusBreakdown.length === 0) {
            <p class="empty-text">No applications yet.</p>
          } @else {
            <div class="status-bars">
              @for (item of data()!.statusBreakdown; track item.status) {
                <div class="status-row">
                  <span class="status-label">{{ item.status }}</span>
                  <div class="bar-track">
                    <div class="bar-fill" [style.width.%]="getBarWidth(item.count)" [class]="'bar-' + item.status.toLowerCase()"></div>
                  </div>
                  <span class="status-count">{{ item.count }}</span>
                </div>
              }
            </div>
          }
        </div>

        <div class="panel">
          <h3 class="panel-title">Verification Stats</h3>
          <div class="verify-grid">
            <div class="verify-item">
              <span class="verify-num">{{ data()!.verificationStats.total }}</span>
              <span class="verify-label">Total</span>
            </div>
            <div class="verify-item green">
              <span class="verify-num">{{ data()!.verificationStats.verified }}</span>
              <span class="verify-label">Verified</span>
            </div>
            <div class="verify-item blue">
              <span class="verify-num">{{ data()!.verificationStats.inProgress }}</span>
              <span class="verify-label">In Progress</span>
            </div>
            <div class="verify-item yellow">
              <span class="verify-num">{{ data()!.verificationStats.pending }}</span>
              <span class="verify-label">Pending</span>
            </div>
          </div>
        </div>
      </div>

      <div class="panel full-width">
        <h3 class="panel-title">Recent Applications</h3>
        @if (data()!.recentApplications.length === 0) {
          <p class="empty-text">No applications yet.</p>
        } @else {
          <div class="table-wrapper">
            <table class="data-table">
              <thead>
                <tr>
                  <th>#</th>
                  <th>Candidate</th>
                  <th>Position</th>
                  <th>Status</th>
                  <th>Applied</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                @for (app of data()!.recentApplications; track app.jobApplicationId) {
                  <tr>
                    <td>{{ app.jobApplicationId }}</td>
                    <td class="fw-500">{{ app.candidateName }}</td>
                    <td>{{ app.jobTitle }}</td>
                    <td><span class="badge" [class]="'badge-' + app.status.toLowerCase()">{{ app.status }}</span></td>
                    <td>{{ app.appliedDate | date:'mediumDate' }}</td>
                    <td><a [routerLink]="['/pipeline', app.jobApplicationId]" class="pipeline-link">View Pipeline</a></td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </div>
    }
  `,
  styles: [`
    .loading-state { text-align: center; padding: 3rem; color: var(--text-secondary, #6b7280); }
    .stats-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 1rem; margin-bottom: 1.5rem; }
    .stat-card { background: var(--bg-primary, #fff); border: 1px solid var(--border-color, #e5e7eb); border-radius: 10px; padding: 1.25rem; text-align: center; }
    .stat-card.accent { background: #ecfdf5; border-color: #a7f3d0; }
    .stat-value { font-size: 2rem; font-weight: 700; color: var(--text-primary, #111827); }
    .stat-card.accent .stat-value { color: #065f46; }
    .stat-label { font-size: 0.813rem; color: var(--text-secondary, #6b7280); margin-top: 0.25rem; }
    .panels { display: grid; grid-template-columns: 1fr 1fr; gap: 1.5rem; margin-bottom: 1.5rem; }
    .panel { background: var(--bg-primary, #fff); border: 1px solid var(--border-color, #e5e7eb); border-radius: 10px; padding: 1.25rem; }
    .panel.full-width { grid-column: 1 / -1; }
    .panel-title { font-size: 1rem; font-weight: 600; margin: 0 0 1rem; color: var(--text-primary, #111827); }
    .empty-text { color: var(--text-secondary, #6b7280); font-size: 0.875rem; }

    .status-bars { display: flex; flex-direction: column; gap: 0.75rem; }
    .status-row { display: flex; align-items: center; gap: 0.75rem; }
    .status-label { width: 110px; font-size: 0.813rem; font-weight: 500; color: var(--text-primary, #111827); text-align: right; }
    .bar-track { flex: 1; height: 22px; background: #f3f4f6; border-radius: 6px; overflow: hidden; }
    .bar-fill { height: 100%; border-radius: 6px; min-width: 8px; transition: width 0.3s; }
    .bar-applied { background: #3b82f6; }
    .bar-screening { background: #8b5cf6; }
    .bar-shortlisted { background: #10b981; }
    .bar-interviewscheduled { background: #f59e0b; }
    .bar-interviewed { background: #f97316; }
    .bar-offered { background: #06b6d4; }
    .bar-hired { background: #059669; }
    .bar-rejected { background: #ef4444; }
    .bar-withdrawn { background: #6b7280; }
    .status-count { font-size: 0.813rem; font-weight: 600; width: 30px; color: var(--text-primary, #111827); }

    .verify-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1rem; }
    .verify-item { text-align: center; padding: 1rem; border-radius: 8px; background: #f9fafb; }
    .verify-item.green { background: #ecfdf5; }
    .verify-item.blue { background: #eff6ff; }
    .verify-item.yellow { background: #fffbeb; }
    .verify-num { display: block; font-size: 1.5rem; font-weight: 700; color: var(--text-primary, #111827); }
    .verify-item.green .verify-num { color: #065f46; }
    .verify-item.blue .verify-num { color: #1e40af; }
    .verify-item.yellow .verify-num { color: #92400e; }
    .verify-label { font-size: 0.75rem; color: var(--text-secondary, #6b7280); }

    .table-wrapper { overflow-x: auto; }
    .data-table { width: 100%; border-collapse: collapse; }
    .data-table th { text-align: left; padding: 0.75rem 1rem; font-size: 0.75rem; font-weight: 600; text-transform: uppercase; color: var(--text-secondary, #6b7280); border-bottom: 1px solid var(--border-color, #e5e7eb); }
    .data-table td { padding: 0.75rem 1rem; font-size: 0.875rem; border-bottom: 1px solid var(--border-color, #e5e7eb); color: var(--text-primary, #111827); }
    .data-table tbody tr:hover { background: var(--bg-hover, #f3f4f6); }
    .fw-500 { font-weight: 500; }
    .badge { padding: 0.2rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 500; }
    .badge-hired { background: #d1fae5; color: #065f46; }
    .badge-offered { background: #cffafe; color: #155e75; }
    .badge-shortlisted { background: #d1fae5; color: #065f46; }
    .badge-applied { background: #dbeafe; color: #1e40af; }
    .badge-rejected { background: #fee2e2; color: #991b1b; }
    .badge-interviewed { background: #ffedd5; color: #9a3412; }
    .badge-interviewscheduled { background: #fef3c7; color: #92400e; }
    .pipeline-link { color: #3b82f6; text-decoration: none; font-weight: 500; font-size: 0.813rem; }
    .pipeline-link:hover { text-decoration: underline; }

    @media (max-width: 768px) {
      .panels { grid-template-columns: 1fr; }
      .stats-grid { grid-template-columns: repeat(2, 1fr); }
    }
  `],
})
export class AnalyticsComponent implements OnInit {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  data = signal<any>(null);
  loading = signal(false);

  ngOnInit(): void {
    this.loading.set(true);
    this.http.get<any>(`${this.baseUrl}/pipeline/analytics`).subscribe({
      next: (res) => {
        this.data.set(res.content ?? res);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  getBarWidth(count: number): number {
    if (!this.data()?.statusBreakdown?.length) return 0;
    const max = Math.max(...this.data().statusBreakdown.map((s: any) => s.count));
    return max > 0 ? (count / max) * 100 : 0;
  }
}
