import { Component, OnInit, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-pre-boarding-list',
  standalone: true,
  imports: [DatePipe, RouterLink],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Pre-Boarding</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Pre-Boarding</h1>
        <p class="page-subtitle">Manage pre-boarding profiles</p>
      </div>
    </div>

    @if (loading()) {
      <div class="loading-state">Loading...</div>
    } @else if (items().length === 0) {
      <div class="empty-state">
        <h3>No pre-boarding profiles found</h3>
        <p>Pre-boarding records will appear here when created from the pipeline.</p>
      </div>
    } @else {
      <div class="table-wrapper">
        <table class="data-table">
          <thead>
            <tr>
              <th>#</th>
              <th>Candidate</th>
              <th>Position</th>
              <th>Profile Status</th>
              <th>Created</th>
              <th>Submitted</th>
              <th>Locked</th>
              <th>App Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (item of items(); track item.preBoardingProfileId) {
              <tr>
                <td>{{ item.preBoardingProfileId }}</td>
                <td class="fw-500">{{ item.candidateName }}</td>
                <td>{{ item.jobTitle }}</td>
                <td><span class="badge" [class]="'badge-' + getStatusClass(item.profileStatus)">{{ item.profileStatus }}</span></td>
                <td>{{ item.createdAt | date:'mediumDate' }}</td>
                <td>{{ item.submittedAt ? (item.submittedAt | date:'mediumDate') : '-' }}</td>
                <td>{{ item.isLocked ? 'Yes' : 'No' }}</td>
                <td><span class="badge badge-default">{{ item.applicationStatus }}</span></td>
                <td><a [routerLink]="['/pipeline', item.jobApplicationId]" class="pipeline-link">View Pipeline</a></td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    }
  `,
  styles: [`
    .table-wrapper { overflow-x: auto; background: var(--bg-primary, #fff); border: 1px solid var(--border-color, #e5e7eb); border-radius: 8px; }
    .data-table { width: 100%; border-collapse: collapse; }
    .data-table th { text-align: left; padding: 0.75rem 1rem; font-size: 0.75rem; font-weight: 600; text-transform: uppercase; color: var(--text-secondary, #6b7280); border-bottom: 1px solid var(--border-color, #e5e7eb); background: var(--bg-secondary, #f9fafb); }
    .data-table td { padding: 0.75rem 1rem; font-size: 0.875rem; border-bottom: 1px solid var(--border-color, #e5e7eb); color: var(--text-primary, #111827); }
    .data-table tbody tr:hover { background: var(--bg-hover, #f3f4f6); }
    .fw-500 { font-weight: 500; }
    .badge { padding: 0.2rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 500; }
    .badge-pending { background: #fef3c7; color: #92400e; }
    .badge-submitted { background: #dbeafe; color: #1e40af; }
    .badge-validated { background: #d1fae5; color: #065f46; }
    .badge-locked { background: #e5e7eb; color: #374151; }
    .badge-default { background: #f3f4f6; color: #374151; }
    .loading-state, .empty-state { text-align: center; padding: 3rem; color: var(--text-secondary, #6b7280); }
    .pipeline-link { color: #3b82f6; text-decoration: none; font-weight: 500; font-size: 0.813rem; }
    .pipeline-link:hover { text-decoration: underline; }
  `],
})
export class PreBoardingListComponent implements OnInit {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  items = signal<any[]>([]);
  loading = signal(false);

  ngOnInit(): void {
    this.loading.set(true);
    this.http.get<any>(`${this.baseUrl}/pipeline/pre-boardings`).subscribe({
      next: (res) => {
        this.items.set(res.content ?? res ?? []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Pending: 'pending', Submitted: 'submitted', Validated: 'validated', Locked: 'locked',
    };
    return map[status] ?? 'default';
  }
}
