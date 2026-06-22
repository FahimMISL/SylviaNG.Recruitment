import { Component, OnInit, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { SlicePipe } from '@angular/common';
import { environment } from '../../../environments/environment';
import { ModalComponent } from '../../shared/components/modal.component';

@Component({
  selector: 'app-candidate-jobs',
  standalone: true,
  imports: [FormsModule, SlicePipe, ModalComponent],
  template: `
    <div class="page-header">
      <div>
        <h1 class="page-title">Browse Jobs</h1>
        <p class="page-subtitle">Find and apply to open positions</p>
      </div>
    </div>

    <div class="search-bar">
      <input
        class="form-control"
        placeholder="Search jobs by title or description..."
        [(ngModel)]="searchTerm"
        (input)="onSearch()" />
    </div>

    @if (loading()) {
      <div class="loading-state">Loading jobs...</div>
    } @else if (jobs().length === 0) {
      <div class="empty-state">
        <div class="empty-icon">📋</div>
        <h3>No open positions found</h3>
        <p>Check back later for new openings.</p>
      </div>
    } @else {
      <div class="jobs-grid">
        @for (job of jobs(); track job.jobPostingId) {
          <div class="job-card">
            <div class="job-card-header">
              <h3 class="job-title">{{ job.title }}</h3>
              <span class="job-type badge" [class]="'badge-' + getTypeClass(job.employmentType)">
                {{ getTypeName(job.employmentType) }}
              </span>
            </div>
            @if (job.location) {
              <div class="job-location">📍 {{ job.location }}</div>
            }
            @if (job.description) {
              <p class="job-desc">{{ job.description | slice:0:200 }}{{ job.description.length > 200 ? '...' : '' }}</p>
            }
            <div class="job-meta">
              @if (job.minSalary || job.maxSalary) {
                <span class="meta-item">💰 {{ formatSalary(job.minSalary, job.maxSalary) }}</span>
              }
              @if (job.numberOfPositions > 1) {
                <span class="meta-item">👥 {{ job.numberOfPositions }} positions</span>
              }
              @if (job.closingDate) {
                <span class="meta-item">📅 Closes: {{ job.closingDate | slice:0:10 }}</span>
              }
            </div>
            <div class="job-card-actions">
              <button class="btn btn-sm btn-outline" (click)="viewDetails(job)">View Details</button>
              @if (appliedJobIds().has(job.jobPostingId)) {
                <span class="applied-badge">✓ Applied</span>
              } @else {
                <button class="btn btn-sm btn-primary" (click)="openApply(job)">Apply Now</button>
              }
            </div>
          </div>
        }
      </div>

      @if (totalCount() > pageSize) {
        <div class="pagination">
          <button class="btn btn-sm btn-outline" [disabled]="currentPage() <= 1" (click)="changePage(currentPage() - 1)">Previous</button>
          <span class="page-info">Page {{ currentPage() }} of {{ totalPages() }}</span>
          <button class="btn btn-sm btn-outline" [disabled]="currentPage() >= totalPages()" (click)="changePage(currentPage() + 1)">Next</button>
        </div>
      }
    }

    <app-modal [open]="showDetailModal()" title="Job Details" size="lg" (close)="showDetailModal.set(false)">
      @if (selectedJob()) {
        <div class="job-detail">
          <h2>{{ selectedJob()!.title }}</h2>
          <div class="detail-badges">
            <span class="badge" [class]="'badge-' + getTypeClass(selectedJob()!.employmentType)">
              {{ getTypeName(selectedJob()!.employmentType) }}
            </span>
            @if (selectedJob()!.numberOfPositions > 1) {
              <span class="badge badge-info">{{ selectedJob()!.numberOfPositions }} positions</span>
            }
          </div>
          @if (selectedJob()!.location) {
            <div class="detail-section">
              <h4>Location</h4>
              <p>📍 {{ selectedJob()!.location }}</p>
            </div>
          }
          @if (selectedJob()!.description) {
            <div class="detail-section">
              <h4>Description</h4>
              <p>{{ selectedJob()!.description }}</p>
            </div>
          }
          @if (selectedJob()!.requirements) {
            <div class="detail-section">
              <h4>Requirements</h4>
              <p>{{ selectedJob()!.requirements }}</p>
            </div>
          }
          @if (selectedJob()!.minSalary || selectedJob()!.maxSalary) {
            <div class="detail-section">
              <h4>Salary Range</h4>
              <p>{{ formatSalary(selectedJob()!.minSalary, selectedJob()!.maxSalary) }}</p>
            </div>
          }
          @if (selectedJob()!.closingDate) {
            <div class="detail-section">
              <h4>Application Deadline</h4>
              <p>{{ selectedJob()!.closingDate | slice:0:10 }}</p>
            </div>
          }
          @if (hasEligibility(selectedJob())) {
            <div class="detail-section eligibility-section">
              <h4>Eligibility Requirements</h4>
              <div class="eligibility-list">
                @if (selectedJob()!.minAge || selectedJob()!.maxAge) {
                  <span class="eligibility-item">🎂 Age: {{ selectedJob()!.minAge ?? '—' }} - {{ selectedJob()!.maxAge ?? '—' }} years</span>
                }
                @if (selectedJob()!.minExperienceYears) {
                  <span class="eligibility-item">💼 Min {{ selectedJob()!.minExperienceYears }} years experience</span>
                }
                @if (selectedJob()!.minEducationLevel) {
                  <span class="eligibility-item">🎓 Min Education: {{ selectedJob()!.minEducationLevel }}</span>
                }
                @if (selectedJob()!.requiredDistrict) {
                  <span class="eligibility-item">📍 District: {{ selectedJob()!.requiredDistrict }}</span>
                }
              </div>
            </div>
          }
          <div class="detail-actions">
            @if (appliedJobIds().has(selectedJob()!.jobPostingId)) {
              <span class="applied-badge">✓ You have already applied</span>
            } @else {
              <button class="btn btn-primary" (click)="openApply(selectedJob()!); showDetailModal.set(false)">Apply Now</button>
            }
          </div>
        </div>
      }
    </app-modal>

    <app-modal [open]="showApplyModal()" title="Apply for Position" size="md" (close)="showApplyModal.set(false)">
      @if (applyingJob()) {
        <div class="apply-form">
          <p class="apply-info">Applying for: <strong>{{ applyingJob()!.title }}</strong></p>

          @if (feeInfo() && !feeInfo()!.isPaid) {
            <div class="fee-notice">
              <p><strong>Application Fee Required</strong></p>
              <p>Amount: <strong>{{ feeInfo()!.amount }} {{ feeInfo()!.currency }}</strong></p>
              <button type="button" class="btn btn-primary" [disabled]="initiatingPayment()" (click)="initiatePayment()">
                {{ initiatingPayment() ? 'Redirecting...' : 'Pay & Apply' }}
              </button>
            </div>
          } @else {
            <div class="form-group">
              <label class="form-label">Cover Letter (optional)</label>
              <textarea class="form-control" [(ngModel)]="coverLetter" rows="5" placeholder="Tell us why you're interested in this position..."></textarea>
            </div>
            @if (feeInfo()?.isPaid) {
              <div style="color: #059669; font-size: 0.875rem; margin-bottom: 0.75rem;">&#10003; Application fee paid</div>
            }
            <div class="form-actions">
              <button type="button" class="btn btn-outline" (click)="showApplyModal.set(false)">Cancel</button>
              <button type="button" class="btn btn-primary" [disabled]="submitting()" (click)="submitApplication()">
                {{ submitting() ? 'Submitting...' : 'Submit Application' }}
              </button>
            </div>
          }
        </div>
      }
    </app-modal>
  `,
  styles: [`
    .search-bar { margin-bottom: 1.5rem; }
    .search-bar .form-control { max-width: 400px; }
    .jobs-grid { display: grid; gap: 1rem; grid-template-columns: repeat(auto-fill, minmax(340px, 1fr)); }
    .job-card { background: var(--bg-primary, #fff); border: 1px solid var(--border-color, #e5e7eb); border-radius: 8px; padding: 1.25rem; transition: box-shadow 0.2s; }
    .job-card:hover { box-shadow: 0 2px 8px rgba(0,0,0,0.08); }
    .job-card-header { display: flex; justify-content: space-between; align-items: flex-start; gap: 0.75rem; margin-bottom: 0.75rem; }
    .job-title { font-size: 1.1rem; font-weight: 600; margin: 0; color: var(--text-primary, #111827); }
    .job-location { font-size: 0.813rem; color: var(--text-secondary, #6b7280); margin-bottom: 0.5rem; }
    .job-desc { color: var(--text-secondary, #6b7280); font-size: 0.875rem; line-height: 1.5; margin-bottom: 0.75rem; }
    .job-meta { display: flex; flex-wrap: wrap; gap: 0.75rem; margin-bottom: 1rem; font-size: 0.813rem; color: var(--text-secondary, #6b7280); }
    .job-card-actions { display: flex; gap: 0.5rem; align-items: center; }
    .applied-badge { color: #059669; font-weight: 600; font-size: 0.875rem; }
    .badge { padding: 0.25rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 500; }
    .badge-info { background: #dbeafe; color: #1e40af; }
    .badge-fulltime { background: #d1fae5; color: #065f46; }
    .badge-parttime { background: #fef3c7; color: #92400e; }
    .badge-contract { background: #e0e7ff; color: #3730a3; }
    .badge-intern { background: #fce7f3; color: #9d174d; }
    .pagination { display: flex; justify-content: center; align-items: center; gap: 1rem; margin-top: 1.5rem; }
    .page-info { font-size: 0.875rem; color: var(--text-secondary, #6b7280); }
    .loading-state { text-align: center; padding: 3rem; color: var(--text-secondary, #6b7280); }
    .empty-state { text-align: center; padding: 3rem; }
    .empty-icon { font-size: 3rem; margin-bottom: 1rem; }
    .empty-state h3 { margin: 0 0 0.5rem; }
    .empty-state p { color: var(--text-secondary, #6b7280); }
    .detail-section { margin: 1rem 0; }
    .detail-section h4 { margin: 0 0 0.25rem; font-size: 0.875rem; color: var(--text-secondary, #6b7280); text-transform: uppercase; }
    .detail-section p { margin: 0; white-space: pre-line; }
    .detail-badges { display: flex; gap: 0.5rem; margin: 0.75rem 0; }
    .detail-actions { margin-top: 1.5rem; padding-top: 1rem; border-top: 1px solid var(--border-color, #e5e7eb); }
    .apply-info { margin-bottom: 1rem; }
    .btn-sm { padding: 0.375rem 0.75rem; font-size: 0.813rem; }
    .eligibility-section { background: #fef3c7; border-radius: 6px; padding: 0.75rem 1rem; }
    .eligibility-list { display: flex; flex-wrap: wrap; gap: 0.5rem; }
    .eligibility-item { font-size: 0.813rem; background: #fff; padding: 0.25rem 0.5rem; border-radius: 4px; border: 1px solid #fbbf24; }
    .fee-notice { background: #fff7ed; border: 1px solid #fed7aa; border-radius: 6px; padding: 1rem; text-align: center; }
    .fee-notice p { margin: 0 0 0.5rem; }
  `],
})
export class CandidateJobsComponent implements OnInit {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  jobs = signal<any[]>([]);
  loading = signal(false);
  totalCount = signal(0);
  currentPage = signal(1);
  pageSize = 12;
  searchTerm = '';
  appliedJobIds = signal<Set<number>>(new Set());

  showDetailModal = signal(false);
  selectedJob = signal<any>(null);
  showApplyModal = signal(false);
  applyingJob = signal<any>(null);
  coverLetter = '';
  submitting = signal(false);
  feeInfo = signal<any>(null);
  initiatingPayment = signal(false);

  totalPages = signal(1);

  private searchTimeout: any;

  ngOnInit(): void {
    this.loadJobs();
    this.loadMyApplications();
  }

  onSearch(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage.set(1);
      this.loadJobs();
    }, 300);
  }

  changePage(page: number): void {
    this.currentPage.set(page);
    this.loadJobs();
  }

  loadJobs(): void {
    this.loading.set(true);
    let url = `${this.baseUrl}/candidate-profile/open-jobs?page=${this.currentPage()}&pageSize=${this.pageSize}`;
    if (this.searchTerm) url += `&search=${encodeURIComponent(this.searchTerm)}`;

    this.http.get<any>(url).subscribe({
      next: (res) => {
        const data = res.content ?? res;
        this.jobs.set(data.items ?? []);
        this.totalCount.set(data.totalCount ?? 0);
        this.totalPages.set(Math.ceil((data.totalCount ?? 0) / this.pageSize));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  loadMyApplications(): void {
    this.http.get<any>(`${this.baseUrl}/candidate-profile/my-applications`).subscribe({
      next: (res) => {
        const apps = res.content ?? res;
        const ids = new Set<number>((apps as any[]).map((a: any) => a.jobPostingId));
        this.appliedJobIds.set(ids);
      },
    });
  }

  viewDetails(job: any): void {
    this.selectedJob.set(job);
    this.showDetailModal.set(true);
  }

  openApply(job: any): void {
    this.applyingJob.set(job);
    this.coverLetter = '';
    this.feeInfo.set(null);
    this.showApplyModal.set(true);

    this.http.get<any>(`${this.baseUrl}/payment/status/${job.jobPostingId}`).subscribe({
      next: (res) => {
        const data = res.content ?? res;
        if (data.hasFee) {
          this.feeInfo.set(data);
        }
      },
    });
  }

  initiatePayment(): void {
    const job = this.applyingJob();
    if (!job) return;
    this.initiatingPayment.set(true);

    this.http.post<any>(`${this.baseUrl}/payment/initiate`, { jobPostingId: job.jobPostingId }).subscribe({
      next: (res) => {
        this.initiatingPayment.set(false);
        const data = res.content ?? res;
        if (data.gatewayUrl) {
          window.location.href = data.gatewayUrl;
        }
      },
      error: () => this.initiatingPayment.set(false),
    });
  }

  submitApplication(): void {
    const job = this.applyingJob();
    if (!job) return;
    this.submitting.set(true);

    this.http.post<any>(`${this.baseUrl}/candidate-profile/apply/${job.jobPostingId}`, {
      coverLetter: this.coverLetter || null,
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.showApplyModal.set(false);
        const ids = new Set(this.appliedJobIds());
        ids.add(job.jobPostingId);
        this.appliedJobIds.set(ids);
      },
      error: () => this.submitting.set(false),
    });
  }

  getTypeName(type: string | number): string {
    const map: Record<string, string> = {
      FullTime: 'Full-Time', PartTime: 'Part-Time', Contract: 'Contract',
      Internship: 'Internship', Freelance: 'Freelance',
    };
    return map[type as string] ?? type?.toString() ?? 'Other';
  }

  getTypeClass(type: string | number): string {
    const map: Record<string, string> = {
      FullTime: 'fulltime', PartTime: 'parttime', Contract: 'contract',
      Internship: 'intern', Freelance: 'contract',
    };
    return map[type as string] ?? 'info';
  }

  hasEligibility(job: any): boolean {
    return !!(job?.minAge || job?.maxAge || job?.minExperienceYears || job?.minEducationLevel || job?.requiredDistrict);
  }

  formatSalary(min?: number, max?: number): string {
    const fmt = (n: number) => n >= 1000 ? `${(n / 1000).toFixed(0)}K` : n.toString();
    if (min && max) return `${fmt(min)} - ${fmt(max)} BDT`;
    if (min) return `From ${fmt(min)} BDT`;
    if (max) return `Up to ${fmt(max)} BDT`;
    return '';
  }
}
