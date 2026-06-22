import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { DataTableComponent, TableColumn, TableAction, PageChangeEvent } from '../../shared/components/data-table.component';
import { ModalComponent } from '../../shared/components/modal.component';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../shared/models/api.models';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [FormsModule, DataTableComponent, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Users & Roles</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Users & Roles</h1>
        <p class="page-subtitle">Manage system users, roles and permissions</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ New</button>
    </div>

    <app-data-table
      [columns]="columns"
      [data]="data()"
      [loading]="loading()"
      [actions]="tableActions"
      searchPlaceholder="Search users & roles..."
      emptyTitle="No users & roles found"
      emptyText="Records will appear here once created."
      (pageChange)="onPageChange($event)" />

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit User' : 'Create User Account'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Full Name <span class="required">*</span></label>
            <input class="form-control" [(ngModel)]="form.fullName" name="fullName" placeholder="e.g. John Doe" required />
          </div>
          <div class="form-group">
            <label class="form-label">Email <span class="required">*</span></label>
            <input class="form-control" type="email" [(ngModel)]="form.email" name="email" placeholder="e.g. john@example.com" required />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Role <span class="required">*</span></label>
            <select class="form-control" [(ngModel)]="form.role" name="role" required>
              <option value="">Select role</option>
              <option value="HR">HR</option>
              <option value="Candidate">Candidate</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Password <span class="required">*</span></label>
            <input class="form-control" type="password" [(ngModel)]="form.password" name="password" placeholder="Min 8 characters" required />
          </div>
        </div>
        @if (!editing()) {
          <div class="alert alert-info" style="font-size: var(--fs-xs); padding: var(--space-3); background: #e8f4fd; color: #0c5460; border: 1px solid #bee5eb; border-radius: var(--radius-md);">
            An email with login credentials will be sent to the user's email address.
          </div>
        }
        <div class="form-actions">
          <button type="button" class="btn btn-outline" (click)="closeModal()">Cancel</button>
          <button type="submit" class="btn btn-primary" [disabled]="saving()">
            {{ saving() ? 'Saving...' : (editing() ? 'Update' : 'Create & Send Email') }}
          </button>
        </div>
      </form>
    </app-modal>
  `,
})
export class UserListComponent implements OnInit {
  columns: TableColumn[] = [
    { key: 'userId', label: 'ID', type: 'number', sortable: true },
    { key: 'username', label: 'Username', type: 'text', sortable: true },
    { key: 'email', label: 'Email', type: 'text', sortable: true },
    { key: 'fullName', label: 'Full Name', type: 'text', sortable: true },
    { key: 'isActive', label: 'Status', type: 'status', sortable: false },
  ];

  tableActions: TableAction[] = [
    { label: 'Edit', handler: (row: any) => this.openEdit(row) },
    { label: 'Delete', class: 'text-danger', handler: (row: any) => this.onDelete(row) },
  ];

  data = signal<PagedResult<any> | null>(null);
  loading = signal(false);
  showModal = signal(false);
  editing = signal(false);
  saving = signal(false);
  editId = 0;

  form: any = this.emptyForm();

  private searchProps = ['username', 'email', 'fullName'];

  constructor(private api: ApiService, private toast: ToastService, private http: HttpClient) {}

  ngOnInit(): void {
    this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
  }

  onPageChange(event: PageChangeEvent): void {
    this.loadData(event);
  }

  loadData(event: PageChangeEvent): void {
    this.loading.set(true);
    this.api.getPaged('user', {
      page: event.page,
      pageSize: event.pageSize,
      sortBy: event.sortBy || undefined,
      sortDirection: event.sortDirection,
      searchTerm: event.searchTerm || undefined,
      searchProperties: event.searchTerm ? this.searchProps : undefined,
    }).subscribe({
      next: (result) => { this.data.set(result); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate(): void {
    this.form = this.emptyForm();
    this.editing.set(false);
    this.editId = 0;
    this.showModal.set(true);
  }

  openEdit(row: any): void {
    this.form = { ...row };
    // Convert dates for input[type=date]
    for (const key of Object.keys(this.form)) {
      if (typeof this.form[key] === 'string' && this.form[key]?.includes('T')) {
        this.form[key] = this.form[key].split('T')[0];
      }
    }
    this.editing.set(true);
    this.editId = row.userId ?? row.id;
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onSave(): void {
    if (!this.form.email || !this.form.fullName || !this.form.role) {
      this.toast.error('Please fill all required fields.');
      return;
    }
    if (!this.editing() && (!this.form.password || this.form.password.length < 8)) {
      this.toast.error('Password must be at least 8 characters.');
      return;
    }

    this.saving.set(true);

    if (this.editing()) {
      const payload = { ...this.form, isActive: true };
      this.api.update('user', this.editId, payload).subscribe({
        next: () => {
          this.toast.success('Updated successfully.');
          this.closeModal();
          this.saving.set(false);
          this.ngOnInit();
        },
        error: () => this.saving.set(false),
      });
    } else {
      this.http.post<any>(`${environment.apiUrl}/account/create`, {
        fullName: this.form.fullName,
        email: this.form.email,
        password: this.form.password,
        role: this.form.role,
      }).subscribe({
        next: (res) => {
          const body = res?.content ?? res;
          if (res?.hasError === false || body?.username) {
            this.toast.success('Account created and credentials emailed.');
          } else {
            this.toast.error(res?.decentMessage ?? 'Failed to create account.');
          }
          this.closeModal();
          this.saving.set(false);
          this.ngOnInit();
        },
        error: () => {
          this.toast.error('Failed to create account.');
          this.saving.set(false);
        },
      });
    }
  }

  onDelete(row: any): void {
    if (!confirm('Are you sure you want to delete this record?')) return;
    this.api.delete('user', row.userId ?? row.id).subscribe({
      next: () => {
        this.toast.success('Deleted successfully.');
        this.ngOnInit();
      },
    });
  }

  private emptyForm(): any {
    return {
      email: '',
      fullName: '',
      role: '',
      password: '',
    };
  }
}
