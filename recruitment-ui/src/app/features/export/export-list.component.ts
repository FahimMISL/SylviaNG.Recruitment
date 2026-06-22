import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DataTableComponent, TableColumn, TableAction, PageChangeEvent } from '../../shared/components/data-table.component';
import { ModalComponent } from '../../shared/components/modal.component';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../shared/models/api.models';

@Component({
  selector: 'app-export-list',
  standalone: true,
  imports: [FormsModule, DataTableComponent, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Export</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Export</h1>
        <p class="page-subtitle">Manage data exports</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ New</button>
    </div>

    <app-data-table
      [columns]="columns"
      [data]="data()"
      [loading]="loading()"
      [actions]="tableActions"
      searchPlaceholder="Search exports..."
      emptyTitle="No exports found"
      emptyText="Records will appear here once created."
      (pageChange)="onPageChange($event)" />

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit Export' : 'Create New Export Request'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Export Type *</label>
            <select class="form-control" [(ngModel)]="form.exportType" name="exportType" required>
              <option value="">Select type</option>
              <option value="Candidates">Candidates</option>
              <option value="Applications">Applications</option>
              <option value="Interviews">Interviews</option>
              <option value="Referrals">Referrals</option>
              <option value="JobPostings">Job Postings</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">File Format *</label>
            <select class="form-control" [(ngModel)]="form.fileFormat" name="fileFormat" required>
              <option value="">Select format</option>
              <option value="Excel">Excel</option>
              <option value="CSV">CSV</option>
              <option value="PDF">PDF</option>
            </select>
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">File Name</label>
            <input class="form-control" [(ngModel)]="form.fileName" name="fileName" placeholder="e.g. candidates-june-2026" />
          </div>
          <div class="form-group">
            <label class="form-label">Status</label>
            <select class="form-control" [(ngModel)]="form.exportStatus" name="exportStatus">
              <option [ngValue]="0">Queued</option>
              <option [ngValue]="1">Processing</option>
              <option [ngValue]="2">Completed</option>
              <option [ngValue]="3">Failed</option>
            </select>
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">Filter Criteria (JSON)</label>
          <textarea class="form-control" [(ngModel)]="form.filterCriteriaJson" name="filterCriteriaJson" rows="2" placeholder='e.g. {"status":"Hired"}'></textarea>
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
export class ExportListComponent implements OnInit {
  private statusLabels: Record<number, string> = { 0: 'Queued', 1: 'Processing', 2: 'Completed', 3: 'Failed' };

  columns: TableColumn[] = [
    { key: 'exportRequestId', label: 'ID', type: 'number', sortable: true },
    { key: 'exportType', label: 'Export Type', type: 'badge', sortable: false },
    { key: 'fileFormat', label: 'Format', type: 'text', sortable: false },
    { key: 'fileName', label: 'File Name', type: 'text', sortable: true },
    { key: 'statusLabel', label: 'Status', type: 'badge', sortable: false },
    { key: 'requestedAt', label: 'Requested', type: 'date', sortable: true },
    { key: 'isActive', label: 'Active', type: 'status', sortable: false },
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

  private searchProps = ['exportType', 'fileName'];

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
  }

  onPageChange(event: PageChangeEvent): void {
    this.loadData(event);
  }

  loadData(event: PageChangeEvent): void {
    this.loading.set(true);
    this.api.getPaged('export-request', {
      page: event.page,
      pageSize: event.pageSize,
      sortBy: event.sortBy || undefined,
      sortDirection: event.sortDirection,
      searchTerm: event.searchTerm || undefined,
      searchProperties: event.searchTerm ? this.searchProps : undefined,
    }).subscribe({
      next: (result) => {
        if (result?.data) {
          (result as any).data = result.data.map((item: any) => ({
            ...item,
            statusLabel: this.statusLabels[item.exportStatus] ?? item.exportStatus,
          }));
        }
        this.data.set(result);
        this.loading.set(false);
      },
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
    this.form = {
      exportType: row.exportType ?? '',
      fileFormat: row.fileFormat ?? '',
      fileName: row.fileName ?? '',
      exportStatus: row.exportStatus ?? 0,
      filterCriteriaJson: row.filterCriteriaJson ?? '',
      requestedByUserId: row.requestedByUserId ?? 0,
    };
    this.editing.set(true);
    this.editId = row.exportRequestId ?? row.id;
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onSave(): void {
    if (!this.form.exportType || !this.form.fileFormat) {
      this.toast.error('Please fill all required fields.');
      return;
    }
    this.saving.set(true);
    const payload = {
      ...this.form,
      requestedByUserId: 3,
      requestedAt: new Date().toISOString(),
      isActive: true,
    };
    const obs = this.editing()
      ? this.api.update('export-request', this.editId, payload)
      : this.api.create('export-request', payload);

    obs.subscribe({
      next: () => {
        this.toast.success(this.editing() ? 'Updated successfully.' : 'Created successfully.');
        this.closeModal();
        this.saving.set(false);
        this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
      },
      error: (err) => {
        this.saving.set(false);
        const msg = err?.error?.decentMessage || err?.error?.errorDetails || 'Failed to save.';
        this.toast.error(msg);
      },
    });
  }

  onDelete(row: any): void {
    if (!confirm('Are you sure you want to delete this record?')) return;
    this.api.delete('export-request', row.exportRequestId ?? row.id).subscribe({
      next: () => {
        this.toast.success('Deleted successfully.');
        this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
      },
    });
  }

  private emptyForm(): any {
    return {
      exportType: '',
      fileFormat: '',
      fileName: '',
      exportStatus: 0,
      filterCriteriaJson: '',
    };
  }
}
