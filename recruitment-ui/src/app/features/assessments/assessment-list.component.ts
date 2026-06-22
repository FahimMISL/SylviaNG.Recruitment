import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DataTableComponent, TableColumn, TableAction, PageChangeEvent } from '../../shared/components/data-table.component';
import { ModalComponent } from '../../shared/components/modal.component';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../shared/models/api.models';

@Component({
  selector: 'app-assessment-list',
  standalone: true,
  imports: [FormsModule, DataTableComponent, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Assessments</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Assessments</h1>
        <p class="page-subtitle">Manage exams and assessments</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ New</button>
    </div>

    <app-data-table
      [columns]="columns"
      [data]="data()"
      [loading]="loading()"
      [actions]="tableActions"
      searchPlaceholder="Search assessments..."
      emptyTitle="No assessments found"
      emptyText="Records will appear here once created."
      (pageChange)="onPageChange($event)" />

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit' : 'Create New'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-row">
        <div class="form-group">
          <label class="form-label">Exam Title *</label>
          <input class="form-control" [(ngModel)]="form.title" name="title" placeholder="e.g. Technical Assessment"required />
        </div>
        <div class="form-group">
          <label class="form-label">Type</label>
          <select class="form-control" [(ngModel)]="form.examType" name="examType">
            <option value="">Select type</option>
              <option value="MCQ">MCQ</option>
              <option value="Written">Written</option>
              <option value="Practical">Practical</option>
              <option value="Mixed">Mixed</option>
          </select>
        </div>
        </div>
        <div class="form-row">
        <div class="form-group">
          <label class="form-label">Duration (min)</label>
          <input class="form-control" type="number" [(ngModel)]="form.duration" name="duration" placeholder="60" />
        </div>
        <div class="form-group">
          <label class="form-label">Total Marks</label>
          <input class="form-control" type="number" [(ngModel)]="form.totalMarks" name="totalMarks" placeholder="100" />
        </div>
        </div>
        <div class="form-row">
        <div class="form-group">
          <label class="form-label">Status</label>
          <select class="form-control" [(ngModel)]="form.status" name="status">
            <option value="">Select status</option>
              <option value="Draft">Draft</option>
              <option value="Active">Active</option>
              <option value="Closed">Closed</option>
          </select>
        </div>
        </div>
        <div class="form-group">
          <label class="form-label">Instructions</label>
          <textarea class="form-control" [(ngModel)]="form.instructions" name="instructions" rows="3" placeholder="Exam instructions..."></textarea>
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
export class AssessmentListComponent implements OnInit {
  columns: TableColumn[] = [
    { key: 'examId', label: 'ID', type: 'number', sortable: true },
    { key: 'title', label: 'Exam Title', type: 'text', sortable: true },
    { key: 'examType', label: 'Type', type: 'badge', sortable: false },
    { key: 'duration', label: 'Duration (min)', type: 'number', sortable: true },
    { key: 'totalMarks', label: 'Total Marks', type: 'number', sortable: true },
    { key: 'status', label: 'Status', type: 'badge', sortable: false },
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
    this.api.getPaged('exam', {
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
    this.editId = row.examId ?? row.id;
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
      ? this.api.update('exam', this.editId, payload)
      : this.api.create('exam', payload);

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
    this.api.delete('exam', row.examId ?? row.id).subscribe({
      next: () => {
        this.toast.success('Deleted successfully.');
        this.ngOnInit();
      },
    });
  }

  private emptyForm(): any {
    return {
      title: '',
      examType: '',
      duration: 0,
      totalMarks: 0,
      status: '',
      instructions: ''
    };
  }
}
