import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DataTableComponent, TableColumn, TableAction, PageChangeEvent } from '../../shared/components/data-table.component';
import { ModalComponent } from '../../shared/components/modal.component';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../shared/models/api.models';

@Component({
  selector: 'app-document-list',
  standalone: true,
  imports: [FormsModule, DataTableComponent, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Documents</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Documents</h1>
        <p class="page-subtitle">Manage document templates</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ New</button>
    </div>

    <app-data-table
      [columns]="columns"
      [data]="data()"
      [loading]="loading()"
      [actions]="tableActions"
      searchPlaceholder="Search documents..."
      emptyTitle="No documents found"
      emptyText="Records will appear here once created."
      (pageChange)="onPageChange($event)" />

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit Document Template' : 'Create New Document Template'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Template Name *</label>
            <input class="form-control" [(ngModel)]="form.templateName" name="templateName" placeholder="e.g. Standard Offer Letter" required />
          </div>
          <div class="form-group">
            <label class="form-label">Document Type *</label>
            <select class="form-control" [(ngModel)]="form.documentType" name="documentType" required>
              <option [ngValue]="0">Offer Letter</option>
              <option [ngValue]="1">Appointment Letter</option>
              <option [ngValue]="2">Medical Letter</option>
              <option [ngValue]="3">Target Letter</option>
              <option [ngValue]="4">Transfer Letter</option>
              <option [ngValue]="5">Offer Summary</option>
              <option [ngValue]="6">Joining Info</option>
              <option [ngValue]="7">Post Joining Info</option>
            </select>
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Version</label>
            <input class="form-control" type="number" [(ngModel)]="form.currentVersion" name="currentVersion" min="1" />
          </div>
          <div class="form-group">
            <label class="form-label">Placeholders (JSON)</label>
            <input class="form-control" [(ngModel)]="form.placeholdersJson" name="placeholdersJson" placeholder='e.g. {"name":"","date":""}' />
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">Description</label>
          <textarea class="form-control" [(ngModel)]="form.description" name="description" rows="3" placeholder="Template description..."></textarea>
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
export class DocumentListComponent implements OnInit {
  private docTypeLabels: Record<number, string> = {
    0: 'Offer Letter', 1: 'Appointment Letter', 2: 'Medical Letter', 3: 'Target Letter',
    4: 'Transfer Letter', 5: 'Offer Summary', 6: 'Joining Info', 7: 'Post Joining Info',
  };

  columns: TableColumn[] = [
    { key: 'documentTemplateId', label: 'ID', type: 'number', sortable: true },
    { key: 'templateName', label: 'Template Name', type: 'text', sortable: true },
    { key: 'docTypeLabel', label: 'Type', type: 'badge', sortable: false },
    { key: 'description', label: 'Description', type: 'text', sortable: true },
    { key: 'currentVersion', label: 'Version', type: 'number', sortable: true },
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

  private searchProps = ['templateName', 'description'];

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
  }

  onPageChange(event: PageChangeEvent): void {
    this.loadData(event);
  }

  loadData(event: PageChangeEvent): void {
    this.loading.set(true);
    this.api.getPaged('document-template', {
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
            docTypeLabel: this.docTypeLabels[item.documentType] ?? item.documentType,
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
      templateName: row.templateName ?? '',
      documentType: row.documentType ?? 0,
      description: row.description ?? '',
      currentVersion: row.currentVersion ?? 1,
      placeholdersJson: row.placeholdersJson ?? '',
    };
    this.editing.set(true);
    this.editId = row.documentTemplateId ?? row.id;
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onSave(): void {
    if (!this.form.templateName) {
      this.toast.error('Please fill all required fields.');
      return;
    }
    this.saving.set(true);
    const payload = { ...this.form, isActive: true };
    const obs = this.editing()
      ? this.api.update('document-template', this.editId, payload)
      : this.api.create('document-template', payload);

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
    this.api.delete('document-template', row.documentTemplateId ?? row.id).subscribe({
      next: () => {
        this.toast.success('Deleted successfully.');
        this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
      },
    });
  }

  private emptyForm(): any {
    return {
      templateName: '',
      documentType: 0,
      description: '',
      currentVersion: 1,
      placeholdersJson: '',
    };
  }
}
