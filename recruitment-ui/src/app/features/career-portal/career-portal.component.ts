import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DataTableComponent, TableColumn, TableAction, PageChangeEvent } from '../../shared/components/data-table.component';
import { ModalComponent } from '../../shared/components/modal.component';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../shared/models/api.models';

@Component({
  selector: 'app-career-portal',
  standalone: true,
  imports: [FormsModule, DataTableComponent, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Career Portal</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Career Portal</h1>
        <p class="page-subtitle">Manage career site content</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ New</button>
    </div>

    <app-data-table
      [columns]="columns"
      [data]="data()"
      [loading]="loading()"
      [actions]="tableActions"
      searchPlaceholder="Search career portal..."
      emptyTitle="No career portal found"
      emptyText="Records will appear here once created."
      (pageChange)="onPageChange($event)" />

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit' : 'Create New'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-row">
        <div class="form-group">
          <label class="form-label">Title *</label>
          <input class="form-control" [(ngModel)]="form.title" name="title" placeholder="Content title"required />
        </div>
        <div class="form-group">
          <label class="form-label">Content Type</label>
          <select class="form-control" [(ngModel)]="form.contentType" name="contentType">
            <option value="">Select content type</option>
              <option value="Page">Page</option>
              <option value="Banner">Banner</option>
              <option value="FAQ">FAQ</option>
              <option value="Testimonial">Testimonial</option>
          </select>
        </div>
        </div>
        <div class="form-group">
          <label class="form-label">Content Body</label>
          <textarea class="form-control" [(ngModel)]="form.body" name="body" rows="3" placeholder="Content body..."></textarea>
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
export class CareerPortalComponent implements OnInit {
  columns: TableColumn[] = [
    { key: 'careerContentId', label: 'ID', type: 'number', sortable: true },
    { key: 'title', label: 'Title', type: 'text', sortable: true },
    { key: 'contentType', label: 'Type', type: 'badge', sortable: false },
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

  private searchProps = ['title'];

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
  }

  onPageChange(event: PageChangeEvent): void {
    this.loadData(event);
  }

  loadData(event: PageChangeEvent): void {
    this.loading.set(true);
    this.api.getPaged('career-content', {
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
    this.editId = row.careerContentId ?? row.id;
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onSave(): void {
    if (!this.form.title) {
      this.toast.error('Please fill all required fields.');
      return;
    }
    this.saving.set(true);
    const payload = { ...this.form, isActive: true };
    const obs = this.editing()
      ? this.api.update('career-content', this.editId, payload)
      : this.api.create('career-content', payload);

    obs.subscribe({
      next: () => {
        this.toast.success(this.editing() ? 'Updated successfully.' : 'Created successfully.');
        this.closeModal();
        this.saving.set(false);
        this.ngOnInit();
      },
      error: () => this.saving.set(false),
    });
  }

  onDelete(row: any): void {
    if (!confirm('Are you sure you want to delete this record?')) return;
    this.api.delete('career-content', row.careerContentId ?? row.id).subscribe({
      next: () => {
        this.toast.success('Deleted successfully.');
        this.ngOnInit();
      },
    });
  }

  private emptyForm(): any {
    return {
      title: '',
      contentType: '',
      body: ''
    };
  }
}
