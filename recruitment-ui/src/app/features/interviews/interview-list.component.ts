import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ModalComponent } from '../../shared/components/modal.component';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../shared/models/api.models';

interface InterviewSession {
  interviewSessionId: number;
  requisitionId: number;
  sessionTitle: string;
  round: string;
  mode: number | string;
  scheduledDate: string;
  durationMinutes: number;
  interviewVenueId: number | null;
  meetingLink: string;
  scorecardId: number | null;
  isActive: boolean;
}

@Component({
  selector: 'app-interview-list',
  standalone: true,
  imports: [FormsModule, ModalComponent, DatePipe],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Interview Scheduling</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Interview Scheduling</h1>
        <p class="page-subtitle">Schedule and manage interview sessions</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ Schedule Interview</button>
    </div>

    <!-- Table with toolbar -->
    <div class="table-container">
      <div class="table-toolbar">
        <div class="table-toolbar-left">
          <div class="table-search">
            <span>&#128269;</span>
            <input
              type="text"
              placeholder="Search interviews..."
              [(ngModel)]="searchTerm"
              (keyup.enter)="onSearch()" />
          </div>
        </div>
        <div class="table-toolbar-right">
          <select class="form-control" style="width:auto" [(ngModel)]="pageSize" (change)="onPageSizeChange()">
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
      } @else if (data() && data()!.data.length > 0) {
        <table class="data-table">
          <thead>
            <tr>
              <th class="sortable" (click)="onSort('sessionTitle')">
                Session Title
                @if (sortBy === 'sessionTitle') {
                  <span>{{ sortDirection === 'asc' ? ' &#9650;' : ' &#9660;' }}</span>
                }
              </th>
              <th>Round</th>
              <th>Mode</th>
              <th class="sortable" (click)="onSort('scheduledDate')">
                Date &amp; Time
                @if (sortBy === 'scheduledDate') {
                  <span>{{ sortDirection === 'asc' ? ' &#9650;' : ' &#9660;' }}</span>
                }
              </th>
              <th>Duration</th>
              <th>Meeting Link</th>
              <th>Status</th>
              <th>Active</th>
              <th style="width: 120px;">Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (row of data()!.data; track row.interviewSessionId) {
              <tr>
                <td style="font-weight: 500;">{{ row.sessionTitle }}</td>
                <td>
                  @if (row.round) {
                    <span class="badge badge-primary">{{ row.round }}</span>
                  } @else {
                    <span style="color: var(--color-gray-400);">--</span>
                  }
                </td>
                <td>
                  <span class="badge" [class]="getModeClass(row.mode)">
                    {{ getModeName(row.mode) }}
                  </span>
                </td>
                <td>
                  @if (row.scheduledDate) {
                    <div>{{ row.scheduledDate | date:'MMM d, y' }}</div>
                    <div style="font-size: 0.85em; color: var(--color-gray-500);">
                      {{ row.scheduledDate | date:'h:mm a' }}
                    </div>
                  } @else {
                    <span style="color: var(--color-gray-400);">Not scheduled</span>
                  }
                </td>
                <td>
                  @if (row.durationMinutes) {
                    {{ row.durationMinutes }} min
                  } @else {
                    --
                  }
                </td>
                <td>
                  @if (row.meetingLink) {
                    <a [href]="row.meetingLink" target="_blank" rel="noopener" style="color: var(--color-accent); text-decoration: none;">
                      Join Link
                    </a>
                  } @else if (row.mode === 0 || row.mode === 'InPerson') {
                    <span style="color: var(--color-gray-400);">In-Person</span>
                  } @else {
                    <span style="color: var(--color-gray-400);">--</span>
                  }
                </td>
                <td>
                  <span class="badge" [class]="getStatusClass(row.scheduledDate)">
                    {{ getStatus(row.scheduledDate) }}
                  </span>
                </td>
                <td>
                  @if (row.isActive) {
                    <span class="badge badge-success">Active</span>
                  } @else {
                    <span class="badge badge-neutral">Inactive</span>
                  }
                </td>
                <td>
                  <div class="action-buttons">
                    <button class="action-btn action-edit" title="Edit" (click)="openEdit(row)">&#9998;</button>
                    <button class="action-btn action-delete" title="Delete" (click)="onDelete(row)">&#128465;</button>
                  </div>
                </td>
              </tr>
            }
          </tbody>
        </table>

        <!-- Pagination -->
        @if (data()!.totalPages > 1) {
          <div class="table-footer">
            <div class="table-info">
              Showing {{ ((data()!.pageNumber - 1) * data()!.pageSize) + 1 }}
              - {{ data()!.pageNumber * data()!.pageSize > data()!.totalCount ? data()!.totalCount : data()!.pageNumber * data()!.pageSize }}
              of {{ data()!.totalCount }} interviews
            </div>
            <div class="table-pagination">
              <button class="btn btn-sm btn-outline" [disabled]="!data()!.hasPreviousPage" (click)="goToPage(data()!.pageNumber - 1)">Prev</button>
              @for (p of getPageNumbers(); track p) {
                <button class="btn btn-sm" [class.btn-primary]="p === data()!.pageNumber" [class.btn-outline]="p !== data()!.pageNumber" (click)="goToPage(p)">{{ p }}</button>
              }
              <button class="btn btn-sm btn-outline" [disabled]="!data()!.hasNextPage" (click)="goToPage(data()!.pageNumber + 1)">Next</button>
            </div>
          </div>
        }
      } @else {
        <div class="card-body" style="text-align: center; padding: 3rem;">
          <h3 style="color: var(--color-gray-500); margin-bottom: 0.5rem;">No interviews found</h3>
          <p style="color: var(--color-gray-400);">Scheduled interviews will appear here once created.</p>
        </div>
      }
    </div>

    <!-- Create/Edit Modal -->
    <app-modal [open]="showModal()" [title]="editing() ? 'Edit Interview Session' : 'Schedule New Interview'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Session Title *</label>
            <input class="form-control" [(ngModel)]="form.sessionTitle" name="sessionTitle" placeholder="e.g. Technical Interview - Round 1" required />
          </div>
          <div class="form-group">
            <label class="form-label">Round</label>
            <input class="form-control" [(ngModel)]="form.round" name="round" placeholder="e.g. Round 1, Technical, HR" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Mode *</label>
            <select class="form-control" [(ngModel)]="form.mode" name="mode" (ngModelChange)="onModeChange()">
              <option [ngValue]="0">In-Person</option>
              <option [ngValue]="1">Virtual</option>
              <option [ngValue]="2">Phone</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Duration *</label>
            <select class="form-control" [(ngModel)]="form.durationMinutes" name="durationMinutes">
              <option [ngValue]="30">30 minutes</option>
              <option [ngValue]="45">45 minutes</option>
              <option [ngValue]="60">60 minutes</option>
              <option [ngValue]="90">90 minutes</option>
            </select>
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Scheduled Date &amp; Time *</label>
            <input class="form-control" type="datetime-local" [(ngModel)]="form.scheduledDate" name="scheduledDate" required />
          </div>
          <div class="form-group">
            <label class="form-label">Requisition ID</label>
            <input class="form-control" type="number" [(ngModel)]="form.requisitionId" name="requisitionId" placeholder="Linked requisition" />
          </div>
        </div>

        @if (form.mode === 1 || form.mode === 2) {
          <div class="form-group">
            <label class="form-label">Meeting Link {{ form.mode === 1 ? '(Virtual)' : '(Phone)' }}</label>
            <input class="form-control" [(ngModel)]="form.meetingLink" name="meetingLink"
              [placeholder]="form.mode === 1 ? 'e.g. https://zoom.us/j/123456' : 'e.g. +1-555-0100 or conference link'" />
          </div>
        }

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Venue ID</label>
            <input class="form-control" type="number" [(ngModel)]="form.interviewVenueId" name="interviewVenueId" placeholder="Interview venue (optional)" />
          </div>
          <div class="form-group">
            <label class="form-label">Scorecard ID</label>
            <input class="form-control" type="number" [(ngModel)]="form.scorecardId" name="scorecardId" placeholder="Linked scorecard (optional)" />
          </div>
        </div>

        <div class="form-actions">
          <button type="button" class="btn btn-outline" (click)="closeModal()">Cancel</button>
          <button type="submit" class="btn btn-primary" [disabled]="saving()">
            {{ saving() ? 'Saving...' : (editing() ? 'Update Session' : 'Schedule Interview') }}
          </button>
        </div>
      </form>
    </app-modal>
  `,
})
export class InterviewListComponent implements OnInit {
  data = signal<PagedResult<InterviewSession> | null>(null);
  loading = signal(false);
  showModal = signal(false);
  editing = signal(false);
  saving = signal(false);
  editId = 0;

  searchTerm = '';
  pageSize = 10;
  currentPage = 1;
  sortBy = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  form = this.emptyForm();

  private searchProps = ['sessionTitle', 'round'];

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadData();
  }

  // --- Mode helpers ---

  getModeName(mode: number | string): string {
    const m = typeof mode === 'string' ? mode.toLowerCase() : mode;
    switch (m) {
      case 0: case 'inperson': return 'In-Person';
      case 1: case 'virtual': return 'Virtual';
      case 2: case 'phone': return 'Phone';
      default: return 'Unknown';
    }
  }

  getModeClass(mode: number | string): string {
    const m = typeof mode === 'string' ? mode.toLowerCase() : mode;
    switch (m) {
      case 0: case 'inperson': return 'badge-info';
      case 1: case 'virtual': return 'badge-accent';
      case 2: case 'phone': return 'badge-warning';
      default: return 'badge-neutral';
    }
  }

  private modeToNumber(mode: number | string): number {
    if (typeof mode === 'number') return mode;
    switch (mode.toLowerCase()) {
      case 'inperson': return 0;
      case 'virtual': return 1;
      case 'phone': return 2;
      default: return 0;
    }
  }

  // --- Status derived from scheduled date ---

  getStatus(scheduledDate: string): string {
    if (!scheduledDate) return 'Unscheduled';
    const scheduled = new Date(scheduledDate);
    const now = new Date();
    return scheduled > now ? 'Upcoming' : 'Completed';
  }

  getStatusClass(scheduledDate: string): string {
    if (!scheduledDate) return 'badge-neutral';
    const scheduled = new Date(scheduledDate);
    const now = new Date();
    return scheduled > now ? 'badge-accent' : 'badge-success';
  }

  // --- Data loading ---

  loadData(): void {
    this.loading.set(true);
    this.api.getPaged<InterviewSession>('interview-session', {
      page: this.currentPage,
      pageSize: this.pageSize,
      sortBy: this.sortBy || undefined,
      sortDirection: this.sortDirection,
      searchTerm: this.searchTerm || undefined,
      searchProperties: this.searchTerm ? this.searchProps : undefined,
    }).subscribe({
      next: (result) => { this.data.set(result); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadData();
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadData();
  }

  onSort(column: string): void {
    if (this.sortBy === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = column;
      this.sortDirection = 'asc';
    }
    this.loadData();
  }

  goToPage(page: number): void {
    this.currentPage = page;
    this.loadData();
  }

  getPageNumbers(): number[] {
    const d = this.data();
    if (!d) return [];
    const total = d.totalPages;
    const current = d.pageNumber;
    const pages: number[] = [];
    const start = Math.max(1, current - 2);
    const end = Math.min(total, current + 2);
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }

  // --- CRUD ---

  openCreate(): void {
    this.form = this.emptyForm();
    this.editing.set(false);
    this.editId = 0;
    this.showModal.set(true);
  }

  openEdit(row: InterviewSession): void {
    this.form = {
      sessionTitle: row.sessionTitle,
      round: row.round || '',
      mode: this.modeToNumber(row.mode),
      scheduledDate: row.scheduledDate ? this.toDatetimeLocalString(row.scheduledDate) : '',
      durationMinutes: row.durationMinutes || 60,
      requisitionId: row.requisitionId || null,
      meetingLink: row.meetingLink || '',
      interviewVenueId: row.interviewVenueId || null,
      scorecardId: row.scorecardId || null,
    };
    this.editing.set(true);
    this.editId = row.interviewSessionId;
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onModeChange(): void {
    // Clear meeting link when switching to InPerson
    if (this.form.mode === 0) {
      this.form.meetingLink = '';
    }
  }

  onSave(): void {
    if (!this.form.sessionTitle?.trim()) {
      this.toast.error('Session title is required.');
      return;
    }
    if (!this.form.scheduledDate) {
      this.toast.error('Please select a scheduled date and time.');
      return;
    }

    this.saving.set(true);

    const payload: any = {
      sessionTitle: this.form.sessionTitle.trim(),
      round: this.form.round?.trim() || '',
      mode: Number(this.form.mode),
      scheduledDate: new Date(this.form.scheduledDate).toISOString(),
      durationMinutes: Number(this.form.durationMinutes),
      meetingLink: this.form.meetingLink?.trim() || '',
      interviewVenueId: this.form.interviewVenueId || null,
      scorecardId: this.form.scorecardId || null,
      isActive: true,
    };

    if (this.form.requisitionId) {
      payload.requisitionId = Number(this.form.requisitionId);
    }

    const obs = this.editing()
      ? this.api.update('interview-session', this.editId, payload)
      : this.api.create('interview-session', payload);

    obs.subscribe({
      next: () => {
        this.toast.success(this.editing() ? 'Interview session updated.' : 'Interview session scheduled.');
        this.closeModal();
        this.saving.set(false);
        this.loadData();
      },
      error: () => this.saving.set(false),
    });
  }

  onDelete(row: InterviewSession): void {
    if (!confirm('Are you sure you want to delete this interview session?')) return;
    this.api.delete('interview-session', row.interviewSessionId).subscribe({
      next: () => {
        this.toast.success('Interview session deleted.');
        this.loadData();
      },
    });
  }

  // --- Helpers ---

  private toDatetimeLocalString(isoDate: string): string {
    const d = new Date(isoDate);
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  }

  private emptyForm() {
    return {
      sessionTitle: '',
      round: '',
      mode: 0 as number,
      scheduledDate: '',
      durationMinutes: 60 as number,
      requisitionId: null as number | null,
      meetingLink: '',
      interviewVenueId: null as number | null,
      scorecardId: null as number | null,
    };
  }
}
