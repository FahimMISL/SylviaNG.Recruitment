import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe, DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult, PagedRequest } from '../../shared/models/api.models';
import { ModalComponent } from '../../shared/components/modal.component';
import { environment } from '../../../environments/environment';

interface JobPosting {
  jobPostingId: number;
  title: string;
  description: string;
  location: string;
  status: string;
  employmentType: string;
  numberOfPositions: number;
  minSalary: number;
  maxSalary: number;
  postingDate: string;
  closingDate: string;
  isActive: boolean;
  requisitionId: number | null;
}

interface ApprovedRequisition {
  requisitionId: number;
  title: string;
  numberOfPositions: number;
}

@Component({
  selector: 'app-job-posting-list',
  standalone: true,
  imports: [FormsModule, DecimalPipe, DatePipe, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Job Postings</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Job Postings</h1>
        <p class="page-subtitle">{{ isCandidate() ? 'Browse and apply for open positions' : 'Manage all job postings and openings' }}</p>
      </div>
      @if (!isCandidate()) {
        <button class="btn btn-primary" (click)="openCreate()">+ New Job Posting</button>
      }
    </div>

    <div class="table-container">
      <div class="table-toolbar">
        <div class="table-toolbar-left">
          <div class="table-search">
            <span>&#128269;</span>
            <input type="text" placeholder="Search job postings..." [(ngModel)]="searchTerm" (keyup.enter)="search()" />
          </div>
        </div>
        <div class="table-toolbar-right">
          <select class="form-control" style="width:auto" [(ngModel)]="pageSize" (change)="loadData()">
            <option [value]="10">10 per page</option>
            <option [value]="25">25 per page</option>
            <option [value]="50">50 per page</option>
          </select>
        </div>
      </div>

      @if (loading()) {
        <div class="card-body">
          @for (i of [1,2,3,4]; track i) {
            <div class="skeleton skeleton-text" [style.width]="(100 - i * 10) + '%'"></div>
          }
        </div>
      } @else if (data()?.data?.length) {
        <table class="data-table">
          <thead>
            <tr>
              <th (click)="sort('title')" style="cursor:pointer">Title</th>
              <th>Location</th>
              <th>Type</th>
              <th (click)="sort('status')" style="cursor:pointer">Status</th>
              <th>Positions</th>
              <th>Salary Range</th>
              <th>Closing Date</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (job of data()!.data; track job.jobPostingId) {
              <tr>
                <td><strong>{{ job.title }}</strong></td>
                <td>{{ job.location || '—' }}</td>
                <td><span class="badge badge-neutral">{{ job.employmentType }}</span></td>
                <td>
                  <span class="badge"
                    [class.badge-success]="job.status === 'Open'"
                    [class.badge-warning]="job.status === 'Draft'"
                    [class.badge-danger]="job.status === 'Closed'"
                    [class.badge-info]="job.status !== 'Open' && job.status !== 'Draft' && job.status !== 'Closed'">
                    {{ job.status }}
                  </span>
                </td>
                <td>{{ job.numberOfPositions }}</td>
                <td>{{ job.minSalary | number }} - {{ job.maxSalary | number }} BDT</td>
                <td>{{ job.closingDate | date:'mediumDate' }}</td>
                <td>
                  <div class="action-buttons">
                    @if (isCandidate()) {
                      <button class="action-btn action-apply" title="Apply" (click)="applyForJob(job)" [disabled]="applying()">&#10003;</button>
                    } @else {
                      <button class="action-btn action-edit" title="Edit" (click)="openEdit(job)">&#9998;</button>
                      <button class="action-btn action-pipeline" title="Pipeline" (click)="openPipeline(job)">&#9881;</button>
                      <button class="action-btn action-delete" title="Delete" (click)="onDelete(job)">&#128465;</button>
                    }
                  </div>
                </td>
              </tr>
            }
          </tbody>
        </table>

        <div class="table-pagination">
          <div class="pagination-info">
            Showing {{ (data()!.pageNumber - 1) * data()!.pageSize + 1 }} to
            {{ Math.min(data()!.pageNumber * data()!.pageSize, data()!.totalCount) }}
            of {{ data()!.totalCount }} entries
          </div>
          <div class="pagination-controls">
            <button class="pagination-btn" [disabled]="!data()!.hasPreviousPage" (click)="goToPage(currentPage - 1)">&lt;</button>
            @for (p of pageNumbers(); track p) {
              <button class="pagination-btn" [class.active]="p === currentPage" (click)="goToPage(p)">{{ p }}</button>
            }
            <button class="pagination-btn" [disabled]="!data()!.hasNextPage" (click)="goToPage(currentPage + 1)">&gt;</button>
          </div>
        </div>
      } @else {
        <div class="empty-state">
          <div class="empty-state-icon">&#9654;</div>
          <div class="empty-state-title">No job postings found</div>
          <div class="empty-state-text">Create your first job posting to start receiving applications.</div>
          <button class="btn btn-primary" (click)="openCreate()">+ New Job Posting</button>
        </div>
      }
    </div>

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit Job Posting' : 'Create Job Posting'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        @if (!editing()) {
          <div class="form-group">
            <label class="form-label">Approved Requisition *</label>
            <select class="form-control" [(ngModel)]="form.requisitionId" name="requisitionId" (change)="onRequisitionSelect()">
              <option [ngValue]="0">Select an approved requisition...</option>
              @for (req of approvedRequisitions(); track req.requisitionId) {
                <option [ngValue]="req.requisitionId">{{ req.title }} ({{ req.numberOfPositions }} positions)</option>
              }
            </select>
            @if (!approvedRequisitions().length) {
              <small style="color:var(--color-warning);margin-top:4px;display:block;">No approved requisitions available. Admin must approve a requisition first.</small>
            }
          </div>
        }
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Title *</label>
            <input class="form-control" [(ngModel)]="form.title" name="title" placeholder="e.g. Senior Software Engineer" required [readonly]="!editing() && form.requisitionId > 0" />
          </div>
          <div class="form-group">
            <label class="form-label">Employment Type</label>
            <select class="form-control" [(ngModel)]="form.employmentType" name="employmentType">
              <option value="FullTime">Full Time</option>
              <option value="PartTime">Part Time</option>
              <option value="Contract">Contract</option>
              <option value="Internship">Internship</option>
            </select>
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Location</label>
            <input class="form-control" [(ngModel)]="form.location" name="location" placeholder="e.g. Dhaka, Bangladesh" />
          </div>
          <div class="form-group">
            <label class="form-label">Skills & Qualifications</label>
            <input class="form-control" [(ngModel)]="form.requirements" name="requirements" placeholder="e.g. Node.js, React, TypeScript" />
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">Description</label>
          <textarea class="form-control" [(ngModel)]="form.description" name="description" rows="4" placeholder="Job description..."></textarea>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Status</label>
            <select class="form-control" [(ngModel)]="form.status" name="status">
              <option value="Draft">Draft</option>
              <option value="Open">Open</option>
              <option value="Closed">Closed</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Positions</label>
            <input class="form-control" type="number" [(ngModel)]="form.numberOfPositions" name="numberOfPositions" min="1" [readonly]="!editing() && form.requisitionId > 0" />
            @if (!editing() && form.requisitionId > 0) {
              <small style="color:var(--color-text-muted);margin-top:2px;display:block;">Auto-filled from requisition</small>
            }
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Min Salary (BDT)</label>
            <input class="form-control" type="number" [(ngModel)]="form.minSalary" name="minSalary" />
          </div>
          <div class="form-group">
            <label class="form-label">Max Salary (BDT)</label>
            <input class="form-control" type="number" [(ngModel)]="form.maxSalary" name="maxSalary" />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Posting Date</label>
            <input class="form-control" type="date" [(ngModel)]="form.postingDate" name="postingDate" />
          </div>
          <div class="form-group">
            <label class="form-label">Closing Date</label>
            <input class="form-control" type="date" [(ngModel)]="form.closingDate" name="closingDate" />
          </div>
        </div>
        <div class="form-row" style="margin-top:16px;">
          <div class="form-group" style="grid-column: 1 / -1;">
            <label class="form-label" style="font-weight:600;color:#c8102e;">Eligibility Requirements (optional)</label>
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Min Age</label>
            <input class="form-control" type="number" [(ngModel)]="form.minAge" name="minAge" min="0" placeholder="e.g. 18" />
          </div>
          <div class="form-group">
            <label class="form-label">Max Age</label>
            <input class="form-control" type="number" [(ngModel)]="form.maxAge" name="maxAge" min="0" placeholder="e.g. 35" />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Min Experience (years)</label>
            <input class="form-control" type="number" [(ngModel)]="form.minExperienceYears" name="minExperienceYears" min="0" placeholder="e.g. 3" />
          </div>
          <div class="form-group">
            <label class="form-label">Min Education Level</label>
            <select class="form-control" [(ngModel)]="form.minEducationLevel" name="minEducationLevel">
              <option [ngValue]="null">-- No requirement --</option>
              <option value="SSC">SSC</option>
              <option value="HSC">HSC</option>
              <option value="Diploma">Diploma</option>
              <option value="Bachelors">Bachelors</option>
              <option value="Masters">Masters</option>
              <option value="PhD">PhD</option>
            </select>
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">Required District</label>
          <input class="form-control" [(ngModel)]="form.requiredDistrict" name="requiredDistrict" placeholder="e.g. Dhaka" />
        </div>
        <div class="form-actions">
          <button type="button" class="btn btn-outline" (click)="closeModal()">Cancel</button>
          <button type="submit" class="btn btn-primary" [disabled]="saving()">
            {{ saving() ? 'Saving...' : (editing() ? 'Update' : 'Create') }}
          </button>
        </div>
      </form>
    </app-modal>
  `,
})
export class JobPostingListComponent implements OnInit {
  data = signal<PagedResult<JobPosting> | null>(null);
  loading = signal(false);
  showModal = signal(false);
  editing = signal(false);
  saving = signal(false);

  currentPage = 1;
  pageSize = 10;
  searchTerm = '';
  sortBy = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  editId = 0;
  Math = Math;

  form: any = this.emptyForm();

  applying = signal(false);
  approvedRequisitions = signal<ApprovedRequisition[]>([]);

  constructor(
    private api: ApiService,
    private toast: ToastService,
    private auth: AuthService,
    private http: HttpClient,
    private router: Router
  ) {}

  isCandidate(): boolean {
    const roles = this.auth.currentUser()?.roles ?? [];
    if (roles.includes('Admin') || roles.includes('HR')) return false;
    return roles.includes('Candidate');
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    const req: PagedRequest = {
      page: this.currentPage,
      pageSize: this.pageSize,
      sortBy: this.sortBy || undefined,
      sortDirection: this.sortDirection,
      searchTerm: this.searchTerm || undefined,
      searchProperties: this.searchTerm ? ['title', 'description'] : undefined,
    };
    this.api.getPaged<JobPosting>('job-posting', req).subscribe({
      next: (result) => { this.data.set(result); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  search(): void { this.currentPage = 1; this.loadData(); }
  sort(column: string): void {
    if (this.sortBy === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = column;
      this.sortDirection = 'asc';
    }
    this.loadData();
  }
  goToPage(page: number): void { this.currentPage = page; this.loadData(); }

  pageNumbers(): number[] {
    const total = this.data()?.totalPages ?? 0;
    const pages: number[] = [];
    const start = Math.max(1, this.currentPage - 2);
    const end = Math.min(total, this.currentPage + 2);
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  }

  openCreate(): void {
    this.form = this.emptyForm();
    this.editing.set(false);
    this.editId = 0;
    this.loadApprovedRequisitions();
    this.showModal.set(true);
  }

  loadApprovedRequisitions(): void {
    this.api.getPaged<any>('requisition', { page: 1, pageSize: 100 }).subscribe({
      next: (res) => {
        const items = res.data ?? [];
        const approved = items
          .filter((r: any) => r.requisitionStatus === 'Approved')
          .map((r: any) => ({ requisitionId: r.requisitionId, title: r.title, numberOfPositions: r.numberOfPositions }));
        this.approvedRequisitions.set(approved);
      },
    });
  }

  onRequisitionSelect(): void {
    const reqId = this.form.requisitionId;
    const req = this.approvedRequisitions().find(r => r.requisitionId === reqId);
    if (req) {
      this.form.title = req.title;
      this.form.numberOfPositions = req.numberOfPositions;
    }
  }

  openEdit(job: JobPosting): void {
    this.form = {
      ...job,
      postingDate: job.postingDate?.split('T')[0] ?? '',
      closingDate: job.closingDate?.split('T')[0] ?? '',
    };
    this.editing.set(true);
    this.editId = job.jobPostingId;
    this.showModal.set(true);
  }

  closeModal(): void { this.showModal.set(false); }

  onSave(): void {
    if (!this.editing() && !this.form.requisitionId) {
      this.toast.error('Please select an approved requisition.');
      return;
    }
    if (!this.form.title) {
      this.toast.error('Title is required.');
      return;
    }
    this.saving.set(true);
    const payload = { ...this.form, isActive: true };
    const obs = this.editing()
      ? this.api.update('job-posting', this.editId, payload)
      : this.api.create('job-posting', payload);

    obs.subscribe({
      next: () => {
        this.toast.success(this.editing() ? 'Job posting updated.' : 'Job posting created.');
        this.closeModal();
        this.saving.set(false);
        this.loadData();
      },
      error: () => this.saving.set(false),
    });
  }

  onDelete(job: JobPosting): void {
    if (!confirm(`Delete "${job.title}"?`)) return;
    this.api.delete('job-posting', job.jobPostingId).subscribe({
      next: () => { this.toast.success('Job posting deleted.'); this.loadData(); },
    });
  }

  openPipeline(job: JobPosting): void {
    this.router.navigate(['/job-postings', job.jobPostingId, 'pipeline']);
  }

  applyForJob(job: JobPosting): void {
    const user = this.auth.currentUser();
    if (!user) return;

    this.applying.set(true);

    // First check candidate profile completeness
    this.api.getPaged<any>('candidate', {
      page: 1, pageSize: 1,
      searchTerm: user.email,
      searchProperties: ['email'],
    }).subscribe({
      next: (res) => {
        if (!res.data?.length) {
          this.applying.set(false);
          this.toast.error('No candidate profile found. Please complete your profile first.');
          this.router.navigate(['/profile']);
          return;
        }

        const candidate = res.data[0];
        const completeness = candidate.profileCompletenessPercent ?? 0;

        if (completeness < 70) {
          this.applying.set(false);
          this.toast.error(`Your profile is ${completeness}% complete. You need at least 70% to apply.`);
          this.router.navigate(['/profile']);
          return;
        }

        // Profile OK — submit application via the secure apply endpoint
        this.http.post<any>(`${environment.apiUrl}/candidate-profile/apply/${job.jobPostingId}`, {}).subscribe({
          next: () => {
            this.applying.set(false);
            this.toast.success(`Applied for "${job.title}" successfully! Check your email for confirmation.`);
          },
          error: (err) => {
            this.applying.set(false);
            const msg = err.error?.message ?? err.error?.decentMessage ?? 'Failed to apply.';
            this.toast.error(msg);
          },
        });
      },
      error: () => {
        this.applying.set(false);
        this.toast.error('Failed to verify profile. Please try again.');
      },
    });
  }

  private emptyForm(): any {
    return {
      requisitionId: 0, siteId: 1, title: '', description: '', location: '', requirements: '', status: 'Draft', employmentType: 'FullTime',
      numberOfPositions: 1, minSalary: 0, maxSalary: 0, postingDate: '', closingDate: '',
      minAge: null, maxAge: null, minExperienceYears: null, minEducationLevel: null, requiredDistrict: '',
    };
  }
}
