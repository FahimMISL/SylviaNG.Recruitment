import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DataTableComponent, TableColumn, TableAction, PageChangeEvent } from '../../shared/components/data-table.component';
import { ModalComponent } from '../../shared/components/modal.component';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../shared/models/api.models';

@Component({
  selector: 'app-referral-list',
  standalone: true,
  imports: [FormsModule, DataTableComponent, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Referrals</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Referrals</h1>
        <p class="page-subtitle">Manage employee referrals</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ New</button>
    </div>

    <app-data-table
      [columns]="columns"
      [data]="data()"
      [loading]="loading()"
      [actions]="tableActions"
      searchPlaceholder="Search referrals..."
      emptyTitle="No referrals found"
      emptyText="Records will appear here once created."
      (pageChange)="onPageChange($event)" />

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit Referral' : 'Create New Referral'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Candidate *</label>
            <select class="form-control" [(ngModel)]="form.candidateId" name="candidateId" required>
              <option [ngValue]="0">Select candidate</option>
              @for (c of candidates(); track c.candidateId) {
                <option [ngValue]="c.candidateId">{{ c.fullName }}</option>
              }
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Job Posting</label>
            <select class="form-control" [(ngModel)]="form.jobPostingId" name="jobPostingId">
              <option [ngValue]="null">None</option>
              @for (j of jobPostings(); track j.jobPostingId) {
                <option [ngValue]="j.jobPostingId">{{ j.title }}</option>
              }
            </select>
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Referrer Name *</label>
            <input class="form-control" [(ngModel)]="form.referrerName" name="referrerName" placeholder="Employee who referred" required />
          </div>
          <div class="form-group">
            <label class="form-label">Referrer Contact</label>
            <input class="form-control" [(ngModel)]="form.referrerContact" name="referrerContact" placeholder="Phone or email" />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Source *</label>
            <select class="form-control" [(ngModel)]="form.source" name="source" required>
              <option [ngValue]="0">Employee</option>
              <option [ngValue]="1">Agency</option>
              <option [ngValue]="2">Direct</option>
              <option [ngValue]="3">Admin</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Status *</label>
            <select class="form-control" [(ngModel)]="form.referralStatus" name="referralStatus" required>
              <option [ngValue]="0">Submitted</option>
              <option [ngValue]="1">Invitation Sent</option>
              <option [ngValue]="2">Profile Completed</option>
              <option [ngValue]="3">Applied</option>
              <option [ngValue]="4">Rejected</option>
            </select>
          </div>
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
export class ReferralListComponent implements OnInit {
  private sourceLabels: Record<number, string> = { 0: 'Employee', 1: 'Agency', 2: 'Direct', 3: 'Admin' };
  private statusLabels: Record<number, string> = { 0: 'Submitted', 1: 'Invitation Sent', 2: 'Profile Completed', 3: 'Applied', 4: 'Rejected' };

  columns: TableColumn[] = [
    { key: 'referralId', label: 'ID', type: 'number', sortable: true },
    { key: 'referrerName', label: 'Referrer', type: 'text', sortable: true },
    { key: 'candidateId', label: 'Candidate ID', type: 'number', sortable: true },
    { key: 'sourceLabel', label: 'Source', type: 'badge', sortable: false },
    { key: 'statusLabel', label: 'Status', type: 'badge', sortable: false },
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
  candidates = signal<any[]>([]);
  jobPostings = signal<any[]>([]);

  form: any = this.emptyForm();

  private searchProps = ['referrerName'];

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
    this.loadDropdowns();
  }

  onPageChange(event: PageChangeEvent): void {
    this.loadData(event);
  }

  loadData(event: PageChangeEvent): void {
    this.loading.set(true);
    this.api.getPaged('referral', {
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
            sourceLabel: this.sourceLabels[item.source] ?? item.source,
            statusLabel: this.statusLabels[item.referralStatus] ?? item.referralStatus,
          }));
        }
        this.data.set(result);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  loadDropdowns(): void {
    this.api.getPaged('candidate', { page: 1, pageSize: 100, sortDirection: 'asc' }).subscribe({
      next: (res) => this.candidates.set(res?.data ?? []),
    });
    this.api.getPaged('job-posting', { page: 1, pageSize: 100, sortDirection: 'asc' }).subscribe({
      next: (res) => this.jobPostings.set(res?.data ?? []),
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
      candidateId: row.candidateId,
      jobPostingId: row.jobPostingId,
      source: row.source,
      referrerName: row.referrerName ?? '',
      referrerContact: row.referrerContact ?? '',
      referralStatus: row.referralStatus,
    };
    this.editing.set(true);
    this.editId = row.referralId ?? row.id;
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onSave(): void {
    if (!this.form.candidateId || !this.form.referrerName) {
      this.toast.error('Please fill all required fields.');
      return;
    }
    this.saving.set(true);
    const payload = { ...this.form, isActive: true };
    const obs = this.editing()
      ? this.api.update('referral', this.editId, payload)
      : this.api.create('referral', payload);

    obs.subscribe({
      next: () => {
        this.toast.success(this.editing() ? 'Updated successfully.' : 'Created successfully.');
        this.closeModal();
        this.saving.set(false);
        this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
      },
      error: (err) => {
        this.saving.set(false);
        const msg = err?.error?.decentMessage || err?.error?.errorDetails || 'Failed to save. Please try again.';
        this.toast.error(msg);
      },
    });
  }

  onDelete(row: any): void {
    if (!confirm('Are you sure you want to delete this record?')) return;
    this.api.delete('referral', row.referralId ?? row.id).subscribe({
      next: () => {
        this.toast.success('Deleted successfully.');
        this.loadData({ page: 1, pageSize: 10, searchTerm: '', sortBy: '', sortDirection: 'asc' });
      },
    });
  }

  private emptyForm(): any {
    return {
      candidateId: 0,
      jobPostingId: null,
      source: 0,
      referrerName: '',
      referrerContact: '',
      referralStatus: 0,
    };
  }
}
