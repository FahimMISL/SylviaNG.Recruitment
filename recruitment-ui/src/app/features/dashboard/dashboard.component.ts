import { Component, OnInit, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HttpClient, HttpContext } from '@angular/common/http';
import { ApiService } from '../../core/services/api.service';
import { KeycloakService } from '../../core/services/keycloak.service';
import { environment } from '../../../environments/environment';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Dashboard</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Dashboard</h1>
        <p class="page-subtitle">{{ isCandidate() ? 'Your application overview' : 'Recruitment overview and key metrics' }}</p>
      </div>
      @if (!isCandidate()) {
        <a routerLink="/requisitions" class="btn btn-primary">+ New Requisition</a>
      }
    </div>

    @if (isCandidate()) {
      <!-- Candidate Dashboard -->
      <div class="stats-grid">
        <div class="stat-card">
          <div>
            <div class="stat-value">{{ candidateStats().applications }}</div>
            <div class="stat-label">My Applications</div>
          </div>
          <div class="stat-icon accent">&#9654;</div>
        </div>
        <div class="stat-card">
          <div>
            <div class="stat-value">{{ candidateStats().openJobs }}</div>
            <div class="stat-label">Open Positions</div>
          </div>
          <div class="stat-icon info">&#9660;</div>
        </div>
      </div>

      <div class="card mb-6">
        <div class="card-header">
          <h3 class="card-title">Quick Links</h3>
        </div>
        <div class="card-body" style="display:flex;gap:12px;flex-wrap:wrap;">
          <a routerLink="/browse-jobs" class="btn btn-primary">Browse Jobs</a>
          <a routerLink="/my-applications" class="btn btn-outline">My Applications</a>
          <a routerLink="/profile" class="btn btn-outline">My Profile</a>
        </div>
      </div>
    } @else {
      <!-- Admin / HR Dashboard -->
      <div class="stats-grid">
        <div class="stat-card">
          <div>
            <div class="stat-value">{{ stats().jobPostings }}</div>
            <div class="stat-label">Job Postings</div>
          </div>
          <div class="stat-icon accent">&#9654;</div>
        </div>
        <div class="stat-card">
          <div>
            <div class="stat-value">{{ stats().candidates }}</div>
            <div class="stat-label">Candidates</div>
          </div>
          <div class="stat-icon info">&#9660;</div>
        </div>
        <div class="stat-card">
          <div>
            <div class="stat-value">{{ stats().requisitions }}</div>
            <div class="stat-label">Requisitions</div>
          </div>
          <div class="stat-icon warning">&#9670;</div>
        </div>
        <div class="stat-card">
          <div>
            <div class="stat-value">{{ stats().interviews }}</div>
            <div class="stat-label">Interviews</div>
          </div>
          <div class="stat-icon success">&#9742;</div>
        </div>
      </div>

      <div class="card mb-6">
        <div class="card-header">
          <h3 class="card-title">Recent Job Postings</h3>
          <a routerLink="/job-postings" class="btn btn-outline btn-sm">View All</a>
        </div>
        @if (loadingJobs()) {
          <div class="card-body">
            @for (i of [1,2,3]; track i) {
              <div class="skeleton skeleton-text" [style.width]="(100 - i * 10) + '%'"></div>
            }
          </div>
        } @else {
          <div class="card-body p-0">
            <table class="data-table">
              <thead>
                <tr>
                  <th>Title</th>
                  <th>Type</th>
                  <th>Positions</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                @for (job of recentJobs(); track job.jobPostingId) {
                  <tr>
                    <td><strong>{{ job.title }}</strong></td>
                    <td><span class="badge badge-neutral">{{ job.employmentType }}</span></td>
                    <td>{{ job.numberOfPositions }}</td>
                    <td>
                      <span class="badge"
                        [class.badge-success]="job.status === 'Open'"
                        [class.badge-warning]="job.status === 'Draft'"
                        [class.badge-danger]="job.status === 'Closed'">
                        {{ job.status }}
                      </span>
                    </td>
                    <td><a routerLink="/job-postings" class="btn btn-ghost btn-sm">View</a></td>
                  </tr>
                } @empty {
                  <tr>
                    <td colspan="5" class="text-center text-muted" style="padding:24px">
                      No job postings yet. Create one to get started.
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </div>

      <div class="card">
        <div class="card-header">
          <h3 class="card-title">Recent Candidates</h3>
          <a routerLink="/candidates" class="btn btn-outline btn-sm">View All</a>
        </div>
        @if (loadingCandidates()) {
          <div class="card-body">
            @for (i of [1,2,3]; track i) {
              <div class="skeleton skeleton-text" [style.width]="(100 - i * 10) + '%'"></div>
            }
          </div>
        } @else {
          <div class="card-body p-0">
            <table class="data-table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Phone</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                @for (c of recentCandidates(); track c.candidateId) {
                  <tr>
                    <td><strong>{{ c.fullName }}</strong></td>
                    <td>{{ c.email }}</td>
                    <td>{{ c.phone }}</td>
                    <td>
                      @if (c.isActive) {
                        <span class="badge badge-success">Active</span>
                      } @else {
                        <span class="badge badge-neutral">Inactive</span>
                      }
                    </td>
                    <td><a routerLink="/candidates" class="btn btn-ghost btn-sm">View</a></td>
                  </tr>
                } @empty {
                  <tr>
                    <td colspan="5" class="text-center text-muted" style="padding:24px">
                      No candidates registered yet.
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      </div>
    }
  `,
})
export class DashboardComponent implements OnInit {
  private api = inject(ApiService);
  private http = inject(HttpClient);
  private auth = inject(KeycloakService);

  isCandidate = signal(false);
  stats = signal({ jobPostings: 0, candidates: 0, requisitions: 0, interviews: 0 });
  candidateStats = signal({ applications: 0, openJobs: 0 });
  recentJobs = signal<any[]>([]);
  recentCandidates = signal<any[]>([]);
  loadingJobs = signal(true);
  loadingCandidates = signal(true);

  ngOnInit(): void {
    const roles = this.auth.currentUser()?.roles ?? [];
    this.isCandidate.set(roles.includes('Candidate') && !roles.includes('Admin') && !roles.includes('HR'));

    if (this.isCandidate()) {
      this.loadCandidateDashboard();
    } else {
      this.loadAdminDashboard();
    }
  }

  private loadCandidateDashboard(): void {
    const ctx = { context: new HttpContext().set(SKIP_ERROR_TOAST, true) };
    this.http.get<any>(`${environment.apiUrl}/candidate-profile/my-applications`, ctx).subscribe({
      next: (res) => {
        const apps = res.content ?? res ?? [];
        this.candidateStats.update(s => ({ ...s, applications: Array.isArray(apps) ? apps.length : 0 }));
      },
      error: () => {},
    });

    this.http.get<any>(`${environment.apiUrl}/candidate-profile/open-jobs?page=1&pageSize=1`, ctx).subscribe({
      next: (res) => {
        const data = res.content ?? res;
        this.candidateStats.update(s => ({ ...s, openJobs: data.totalCount ?? 0 }));
      },
      error: () => {},
    });
  }

  private loadAdminDashboard(): void {
    this.api.getPaged<any>('job-posting', { page: 1, pageSize: 5 }).subscribe({
      next: (result) => {
        this.recentJobs.set(result.data);
        this.stats.update((s) => ({ ...s, jobPostings: result.totalCount }));
        this.loadingJobs.set(false);
      },
      error: () => this.loadingJobs.set(false),
    });

    this.api.getPaged<any>('candidate', { page: 1, pageSize: 5 }).subscribe({
      next: (result) => {
        this.recentCandidates.set(result.data);
        this.stats.update((s) => ({ ...s, candidates: result.totalCount }));
        this.loadingCandidates.set(false);
      },
      error: () => this.loadingCandidates.set(false),
    });

    this.api.getPaged<any>('requisition', { page: 1, pageSize: 1 }).subscribe({
      next: (result) => this.stats.update((s) => ({ ...s, requisitions: result.totalCount })),
      error: () => {},
    });

    this.api.getPaged<any>('interview-session', { page: 1, pageSize: 1 }).subscribe({
      next: (result) => this.stats.update((s) => ({ ...s, interviews: result.totalCount })),
      error: () => {},
    });
  }
}
