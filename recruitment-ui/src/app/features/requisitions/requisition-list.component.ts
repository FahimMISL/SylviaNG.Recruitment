import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult, PagedRequest } from '../../shared/models/api.models';
import { ModalComponent } from '../../shared/components/modal.component';

interface Requisition {
  requisitionId: number;
  title: string;
  jobDescription: string;
  requisitionStatus: string;
  requisitionType: string;
  budgetCode: string;
  roleCategory: string;
  numberOfPositions: number;
  approvedAt: string;
  createdAt: string;
  isActive: boolean;
  [key: string]: any;
}

@Component({
  selector: 'app-requisition-list',
  standalone: true,
  imports: [FormsModule, DatePipe, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Requisitions</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Requisitions</h1>
        <p class="page-subtitle">Manage hiring requisitions and approvals</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ New Requisition</button>
    </div>

    <div class="table-container">
      <div class="table-toolbar">
        <div class="table-toolbar-left">
          <div class="table-search">
            <span>&#128269;</span>
            <input type="text" placeholder="Search requisitions..." [(ngModel)]="searchTerm" (keyup.enter)="search()" />
          </div>
        </div>
      </div>

      @if (loading()) {
        <div class="card-body">
          @for (i of [1,2,3]; track i) {
            <div class="skeleton skeleton-text" [style.width]="(100 - i * 10) + '%'"></div>
          }
        </div>
      } @else if (data()?.data?.length) {
        <table class="data-table">
          <thead>
            <tr>
              <th>Title</th>
              <th>Department</th>
              <th>Status</th>
              <th>Priority</th>
              <th>Positions</th>
              <th>Requested</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (r of data()!.data; track r.requisitionId) {
              <tr>
                <td><strong>{{ r.title }}</strong></td>
                <td>{{ r.roleCategory || '—' }}</td>
                <td>
                  <span class="badge"
                    [class.badge-success]="r.requisitionStatus === 'Approved'"
                    [class.badge-warning]="r.requisitionStatus === 'Submitted'"
                    [class.badge-danger]="r.requisitionStatus === 'Rejected'"
                    [class.badge-info]="r.requisitionStatus === 'Draft'">
                    {{ r.requisitionStatus }}
                  </span>
                </td>
                <td>
                  <span class="badge"
                    [class.badge-danger]="r.budgetCode === 'High'"
                    [class.badge-warning]="r.budgetCode === 'Medium'"
                    [class.badge-neutral]="r.budgetCode === 'Low'">
                    {{ r.budgetCode || '—' }}
                  </span>
                </td>
                <td>{{ r.numberOfPositions }}</td>
                <td>{{ r.createdAt | date:'mediumDate' }}</td>
                <td>
                  <div class="action-buttons">
                    @if (isAdmin() && r.requisitionStatus === 'Submitted') {
                      <button class="action-btn action-approve" title="Approve" (click)="onApprove(r)">&#10003;</button>
                      <button class="action-btn action-reject" title="Reject" (click)="onReject(r)">&#10007;</button>
                    }
                    <button class="action-btn action-edit" title="Edit" (click)="openEdit(r)">&#9998;</button>
                    <button class="action-btn action-delete" title="Delete" (click)="onDelete(r)">&#128465;</button>
                  </div>
                </td>
              </tr>
            }
          </tbody>
        </table>

        <div class="table-pagination">
          <div class="pagination-info">
            Page {{ data()!.pageNumber }} of {{ data()!.totalPages }} ({{ data()!.totalCount }} total)
          </div>
          <div class="pagination-controls">
            <button class="pagination-btn" [disabled]="!data()!.hasPreviousPage" (click)="goToPage(currentPage - 1)">&lt;</button>
            <button class="pagination-btn active">{{ currentPage }}</button>
            <button class="pagination-btn" [disabled]="!data()!.hasNextPage" (click)="goToPage(currentPage + 1)">&gt;</button>
          </div>
        </div>
      } @else {
        <div class="empty-state">
          <div class="empty-state-icon">&#9670;</div>
          <div class="empty-state-title">No requisitions found</div>
          <div class="empty-state-text">Create a hiring requisition to start the recruitment process.</div>
          <button class="btn btn-primary" (click)="openCreate()">+ New Requisition</button>
        </div>
      }
    </div>

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit Requisition' : 'New Requisition'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-group">
          <label class="form-label">Title *</label>
          <input class="form-control" [(ngModel)]="form.title" name="title" placeholder="e.g. Software Engineer - Backend" required />
        </div>
        <div class="form-group">
          <label class="form-label">Description</label>
          <textarea class="form-control" [(ngModel)]="form.description" name="description" rows="3" placeholder="Requisition details..."></textarea>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Department</label>
            <input class="form-control" [(ngModel)]="form.department" name="department" placeholder="e.g. Engineering" />
          </div>
          <div class="form-group">
            <label class="form-label">Positions</label>
            <input class="form-control" type="number" [(ngModel)]="form.numberOfPositions" name="numberOfPositions" min="1" />
          </div>
        </div>
        <div class="form-row">
          @if (!isAdmin()) {
            <div class="form-group">
              <label class="form-label">Status</label>
              <select class="form-control" [(ngModel)]="form.status" name="status">
                <option value="Draft">Draft</option>
                <option value="Pending">Pending (Submit for Approval)</option>
              </select>
            </div>
          }
          <div class="form-group">
            <label class="form-label">Priority</label>
            <select class="form-control" [(ngModel)]="form.priority" name="priority">
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
            </select>
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">Requested Date</label>
          <input class="form-control" type="date" [(ngModel)]="form.requestedDate" name="requestedDate" />
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
export class RequisitionListComponent implements OnInit {
  data = signal<PagedResult<Requisition> | null>(null);
  loading = signal(false);
  showModal = signal(false);
  editing = signal(false);
  saving = signal(false);
  currentPage = 1;
  pageSize = 10;
  searchTerm = '';
  editId = 0;

  form: any = this.emptyForm();

  isAdmin = computed(() => this.auth.currentUser()?.roles?.includes('Admin') ?? false);

  constructor(private api: ApiService, private auth: AuthService, private toast: ToastService) {}

  ngOnInit(): void { this.loadData(); }

  loadData(): void {
    this.loading.set(true);
    const req: PagedRequest = {
      page: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      searchProperties: this.searchTerm ? ['title', 'department'] : undefined,
    };
    this.api.getPaged<Requisition>('requisition', req).subscribe({
      next: (result) => { this.data.set(result); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  search(): void { this.currentPage = 1; this.loadData(); }
  goToPage(page: number): void { this.currentPage = page; this.loadData(); }

  openCreate(): void {
    this.form = this.emptyForm();
    this.editing.set(false);
    this.editId = 0;
    this.showModal.set(true);
  }

  openEdit(r: any): void {
    const reverseStatus: Record<string, string> = { Draft: 'Draft', Submitted: 'Pending', Approved: 'Approved', Rejected: 'Rejected' };
    this.form = {
      title: r.title,
      description: r.jobDescription || '',
      department: r.roleCategory || '',
      numberOfPositions: r.numberOfPositions,
      status: reverseStatus[r.requisitionStatus] || 'Draft',
      priority: r.budgetCode || 'Medium',
      requestedDate: '',
    };
    this.editing.set(true);
    this.editId = r.requisitionId;
    this.showModal.set(true);
  }

  closeModal(): void { this.showModal.set(false); }

  onSave(): void {
    if (!this.form.title) {
      this.toast.error('Title is required.');
      return;
    }
    this.saving.set(true);
    const statusMap: Record<string, string> = { Draft: 'Draft', Pending: 'Submitted', Approved: 'Approved', Rejected: 'Rejected' };
    const resolvedStatus = this.isAdmin() ? 'Approved' : (statusMap[this.form.status] || 'Draft');
    const payload = {
      title: this.form.title,
      jobDescription: this.form.description || null,
      requisitionStatus: resolvedStatus,
      requisitionType: 'NewPosition',
      numberOfPositions: this.form.numberOfPositions || 1,
      roleCategory: this.form.department || null,
      budgetCode: this.form.priority || null,
      siteId: null,
      requestedByUserId: null,
      isActive: true,
    };
    const obs = this.editing()
      ? this.api.update('requisition', this.editId, payload)
      : this.api.create('requisition', payload);

    obs.subscribe({
      next: () => {
        this.toast.success(this.editing() ? 'Requisition updated.' : 'Requisition created.');
        this.closeModal();
        this.saving.set(false);
        this.loadData();
      },
      error: () => this.saving.set(false),
    });
  }

  onApprove(r: Requisition): void {
    if (!confirm(`Approve "${r.title}"?`)) return;
    this.api.update('requisition', r.requisitionId, { requisitionStatus: 'Approved' }).subscribe({
      next: () => { this.toast.success('Requisition approved.'); this.loadData(); },
      error: () => this.toast.error('Failed to approve requisition.'),
    });
  }

  onReject(r: Requisition): void {
    if (!confirm(`Reject "${r.title}"?`)) return;
    this.api.update('requisition', r.requisitionId, { requisitionStatus: 'Rejected' }).subscribe({
      next: () => { this.toast.success('Requisition rejected.'); this.loadData(); },
      error: () => this.toast.error('Failed to reject requisition.'),
    });
  }

  onDelete(r: Requisition): void {
    if (!confirm(`Delete "${r.title}"?`)) return;
    this.api.delete('requisition', r.requisitionId).subscribe({
      next: () => { this.toast.success('Requisition deleted.'); this.loadData(); },
    });
  }

  private emptyForm(): any {
    return {
      title: '', description: '', department: '', status: 'Draft',
      priority: 'Medium', numberOfPositions: 1, requestedDate: '',
    };
  }
}
