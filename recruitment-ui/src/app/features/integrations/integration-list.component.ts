import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DataTableComponent, TableColumn, TableAction, PageChangeEvent } from '../../shared/components/data-table.component';
import { ModalComponent } from '../../shared/components/modal.component';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../shared/models/api.models';

@Component({
  selector: 'app-integration-list',
  standalone: true,
  imports: [FormsModule, DataTableComponent, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Integrations</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Integrations</h1>
        <p class="page-subtitle">Manage integration configs</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ New</button>
    </div>

    <app-data-table
      [columns]="columns"
      [data]="data()"
      [loading]="loading()"
      [actions]="tableActions"
      searchPlaceholder="Search integrations..."
      emptyTitle="No integrations found"
      emptyText="Records will appear here once created."
      (pageChange)="onPageChange($event)" />

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit' : 'Create New'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-row">
        <div class="form-group">
          <label class="form-label">Integration Name *</label>
          <input class="form-control" [(ngModel)]="form.name" name="name" placeholder="e.g. HRIS Sync"required />
        </div>
        <div class="form-group">
          <label class="form-label">Type</label>
          <select class="form-control" [(ngModel)]="form.integrationType" name="integrationType">
            <option value="">Select type</option>
              <option value="API">API</option>
              <option value="Webhook">Webhook</option>
              <option value="FileSync">FileSync</option>
              <option value="OAuth">OAuth</option>
          </select>
        </div>
        </div>
        <div class="form-row">
        <div class="form-group">
          <label class="form-label">Endpoint URL</label>
          <input class="form-control" [(ngModel)]="form.endpoint" name="endpoint" placeholder="https://..." />
        </div>
        </div>
        <div class="form-group">
          <label class="form-label">Description</label>
          <textarea class="form-control" [(ngModel)]="form.description" name="description" rows="3" placeholder="Integration details..."></textarea>
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
export class IntegrationListComponent implements OnInit {
  columns: TableColumn[] = [
    { key: 'integrationConfigId', label: 'ID', type: 'number', sortable: true },
    { key: 'name', label: 'Integration Name', type: 'text', sortable: true },
    { key: 'integrationType', label: 'Type', type: 'badge', sortable: false },
    { key: 'endpoint', label: 'Endpoint', type: 'text', sortable: true },
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

  private searchProps = ['name', 'endpoint'];

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
  }

  onPageChange(event: PageChangeEvent): void {
    this.loadData(event);
  }

  loadData(event: PageChangeEvent): void {
    this.loading.set(true);
    this.api.getPaged('integration-config', {
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
    this.editId = row.integrationConfigId ?? row.id;
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onSave(): void {
    if (!this.form.name) {
      this.toast.error('Please fill all required fields.');
      return;
    }
    this.saving.set(true);
    const payload = { ...this.form, isActive: true };
    const obs = this.editing()
      ? this.api.update('integration-config', this.editId, payload)
      : this.api.create('integration-config', payload);

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
    this.api.delete('integration-config', row.integrationConfigId ?? row.id).subscribe({
      next: () => {
        this.toast.success('Deleted successfully.');
        this.ngOnInit();
      },
    });
  }

  private emptyForm(): any {
    return {
      name: '',
      integrationType: '',
      endpoint: '',
      description: ''
    };
  }
}
