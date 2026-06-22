import { Component, OnInit, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { SlicePipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-application-list',
  standalone: true,
  imports: [FormsModule, SlicePipe, DatePipe, RouterLink],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Applications</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Applications</h1>
        <p class="page-subtitle">Track and manage job applications</p>
      </div>
    </div>

    <div class="toolbar">
      <input class="form-control search-input" placeholder="Search by name or email..."
        [(ngModel)]="searchTerm" (input)="onSearch()" />
      <select class="form-control page-select" [(ngModel)]="pageSize" (change)="loadData()">
        <option [value]="10">10 per page</option>
        <option [value]="25">25 per page</option>
        <option [value]="50">50 per page</option>
      </select>
    </div>

    @if (loading()) {
      <div class="loading-state">Loading applications...</div>
    } @else if (applications().length === 0) {
      <div class="empty-state">
        <div class="empty-icon">📋</div>
        <h3>No applications found</h3>
        <p>Applications will appear here when candidates apply for jobs.</p>
      </div>
    } @else {
      <div class="table-wrapper">
        <table class="data-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Candidate</th>
              <th>Email</th>
              <th>Job Position</th>
              <th>Status</th>
              <th>Applied Date</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (app of applications(); track app.jobApplicationId) {
              <tr>
                <td>{{ app.jobApplicationId }}</td>
                <td class="fw-500">{{ app.candidateName }}</td>
                <td>{{ app.candidateEmail }}</td>
                <td>{{ app.jobPostingTitle || 'N/A' }}</td>
                <td><span class="badge" [class]="'badge-' + getStatusClass(app.applicationStatus)">{{ getStatusLabel(app.applicationStatus) }}</span></td>
                <td>{{ app.appliedDate | date:'mediumDate' }}</td>
                <td><a [routerLink]="['/pipeline', app.jobApplicationId]" class="pipeline-link">Pipeline</a></td>
              </tr>
            }
          </tbody>
        </table>
      </div>

      @if (totalCount() > pageSize) {
        <div class="pagination">
          <button class="btn btn-sm btn-outline" [disabled]="currentPage() <= 1" (click)="changePage(currentPage() - 1)">Previous</button>
          <span class="page-info">Page {{ currentPage() }} of {{ totalPages() }}</span>
          <button class="btn btn-sm btn-outline" [disabled]="currentPage() >= totalPages()" (click)="changePage(currentPage() + 1)">Next</button>
        </div>
      }
    }
  `,
  styles: [`
    .toolbar { display: flex; gap: 1rem; margin-bottom: 1.5rem; align-items: center; }
    .search-input { max-width: 300px; }
    .page-select { max-width: 150px; }
    .table-wrapper { overflow-x: auto; background: var(--bg-primary, #fff); border: 1px solid var(--border-color, #e5e7eb); border-radius: 8px; }
    .data-table { width: 100%; border-collapse: collapse; }
    .data-table th { text-align: left; padding: 0.75rem 1rem; font-size: 0.75rem; font-weight: 600; text-transform: uppercase; color: var(--text-secondary, #6b7280); border-bottom: 1px solid var(--border-color, #e5e7eb); background: var(--bg-secondary, #f9fafb); }
    .data-table td { padding: 0.75rem 1rem; font-size: 0.875rem; border-bottom: 1px solid var(--border-color, #e5e7eb); color: var(--text-primary, #111827); }
    .data-table tbody tr:hover { background: var(--bg-hover, #f3f4f6); }
    .fw-500 { font-weight: 500; }
    .badge { padding: 0.2rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 500; }
    .badge-applied { background: #dbeafe; color: #1e40af; }
    .badge-shortlisted { background: #d1fae5; color: #065f46; }
    .badge-rejected { background: #fee2e2; color: #991b1b; }
    .badge-hired { background: #d1fae5; color: #065f46; }
    .badge-default { background: #f3f4f6; color: #374151; }
    .pagination { display: flex; justify-content: center; align-items: center; gap: 1rem; margin-top: 1.5rem; }
    .page-info { font-size: 0.875rem; color: var(--text-secondary, #6b7280); }
    .loading-state { text-align: center; padding: 3rem; color: var(--text-secondary, #6b7280); }
    .empty-state { text-align: center; padding: 3rem; }
    .empty-icon { font-size: 3rem; margin-bottom: 1rem; }
    .empty-state h3 { margin: 0 0 0.5rem; }
    .empty-state p { color: var(--text-secondary, #6b7280); }
    .btn-sm { padding: 0.375rem 0.75rem; font-size: 0.813rem; }
    .pipeline-link { color: #3b82f6; text-decoration: none; font-weight: 500; font-size: 0.813rem; }
    .pipeline-link:hover { text-decoration: underline; }
  `],
})
export class ApplicationListComponent implements OnInit {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  applications = signal<any[]>([]);
  loading = signal(false);
  totalCount = signal(0);
  currentPage = signal(1);
  totalPages = signal(1);
  pageSize = 10;
  searchTerm = '';
  private searchTimeout: any;

  ngOnInit(): void {
    this.loadData();
  }

  onSearch(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage.set(1);
      this.loadData();
    }, 300);
  }

  changePage(page: number): void {
    this.currentPage.set(page);
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    let url = `${this.baseUrl}/job-application/paged?pageNumber=${this.currentPage()}&pageSize=${this.pageSize}`;
    if (this.searchTerm) url += `&searchTerm=${encodeURIComponent(this.searchTerm)}&searchProperties=candidateName,candidateEmail`;

    this.http.get<any>(url).subscribe({
      next: (res) => {
        const data = res.content ?? res;
        this.applications.set(data.data ?? []);
        this.totalCount.set(data.totalCount ?? 0);
        this.totalPages.set(Math.ceil((data.totalCount ?? 0) / this.pageSize));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  getStatusLabel(status: string | number): string {
    const map: Record<string, string> = {
      Applied: 'Applied', Screening: 'Screening', Shortlisted: 'Shortlisted',
      InterviewScheduled: 'Interview Scheduled', Interviewed: 'Interviewed',
      Offered: 'Offered', Hired: 'Hired', Rejected: 'Rejected', Withdrawn: 'Withdrawn',
    };
    return map[status as string] ?? status?.toString() ?? 'Unknown';
  }

  getStatusClass(status: string | number): string {
    const map: Record<string, string> = {
      Applied: 'applied', Shortlisted: 'shortlisted', Rejected: 'rejected', Hired: 'hired',
      InterviewScheduled: 'shortlisted', Interviewed: 'shortlisted', Offered: 'shortlisted',
    };
    return map[status as string] ?? 'default';
  }
}
