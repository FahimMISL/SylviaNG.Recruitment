import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult, PagedRequest } from '../../shared/models/api.models';
import { ModalComponent } from '../../shared/components/modal.component';

interface Candidate {
  candidateId: number;
  fullName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  gender: string;
  address: string;
  city: string;
  country: string;
  linkedInUrl: string;
  resumeUrl: string;
  isActive: boolean;
}

@Component({
  selector: 'app-candidate-list',
  standalone: true,
  imports: [FormsModule, ModalComponent],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Candidates</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Candidates</h1>
        <p class="page-subtitle">Manage candidate profiles and registrations</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ Register Candidate</button>
    </div>

    <div class="table-container">
      <div class="table-toolbar">
        <div class="table-toolbar-left">
          <div class="table-search">
            <span>&#128269;</span>
            <input type="text" placeholder="Search candidates..." [(ngModel)]="searchTerm" (keyup.enter)="search()" />
          </div>
        </div>
      </div>

      @if (loading()) {
        <div class="card-body">
          <div class="skeleton skeleton-text" style="width:100%"></div>
          <div class="skeleton skeleton-text" style="width:80%"></div>
          <div class="skeleton skeleton-text" style="width:90%"></div>
        </div>
      } @else if (data()?.data?.length) {
        <table class="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Phone</th>
              <th>Gender</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (c of data()!.data; track c.candidateId) {
              <tr>
                <td><strong>{{ c.fullName }}</strong></td>
                <td>{{ c.email }}</td>
                <td>{{ c.phone }}</td>
                <td>{{ c.gender }}</td>
                <td>
                  @if (c.isActive) {
                    <span class="badge badge-success">Active</span>
                  } @else {
                    <span class="badge badge-neutral">Inactive</span>
                  }
                </td>
                <td>
                  <div class="action-buttons">
                    <button class="action-btn action-edit" title="Edit" (click)="openEdit(c)">&#9998;</button>
                    <button class="action-btn action-delete" title="Delete" (click)="onDelete(c)">&#128465;</button>
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
          <div class="empty-state-icon">&#9679;</div>
          <div class="empty-state-title">No candidates found</div>
          <div class="empty-state-text">Candidates will appear here once they register.</div>
        </div>
      }
    </div>

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit Candidate' : 'Register Candidate'" size="lg" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-group">
          <label class="form-label">Full Name *</label>
          <input class="form-control" [(ngModel)]="form.fullName" name="fullName" required />
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Email *</label>
            <input class="form-control" type="email" [(ngModel)]="form.email" name="email" required />
          </div>
          <div class="form-group">
            <label class="form-label">Phone</label>
            <input class="form-control" [(ngModel)]="form.phone" name="phone" />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Date of Birth</label>
            <input class="form-control" type="date" [(ngModel)]="form.dateOfBirth" name="dateOfBirth" />
          </div>
          <div class="form-group">
            <label class="form-label">Gender</label>
            <select class="form-control" [(ngModel)]="form.gender" name="gender">
              <option value="">Select gender</option>
              <option value="Male">Male</option>
              <option value="Female">Female</option>
              <option value="Other">Other</option>
            </select>
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">City</label>
            <input class="form-control" [(ngModel)]="form.city" name="city" />
          </div>
          <div class="form-group">
            <label class="form-label">Country</label>
            <input class="form-control" [(ngModel)]="form.country" name="country" />
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">Address</label>
          <textarea class="form-control" [(ngModel)]="form.address" name="address" rows="2"></textarea>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">LinkedIn URL</label>
            <input class="form-control" [(ngModel)]="form.linkedInUrl" name="linkedInUrl" />
          </div>
          <div class="form-group">
            <label class="form-label">Resume URL</label>
            <input class="form-control" [(ngModel)]="form.resumeUrl" name="resumeUrl" />
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
export class CandidateListComponent implements OnInit {
  data = signal<PagedResult<Candidate> | null>(null);
  loading = signal(false);
  showModal = signal(false);
  editing = signal(false);
  saving = signal(false);
  currentPage = 1;
  pageSize = 10;
  searchTerm = '';
  editId = 0;

  form: any = this.emptyForm();

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    const req: PagedRequest = {
      page: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm || undefined,
      searchProperties: this.searchTerm ? ['fullName', 'email'] : undefined,
    };
    this.api.getPaged<Candidate>('candidate', req).subscribe({
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

  openEdit(c: Candidate): void {
    this.form = { ...c, dateOfBirth: c.dateOfBirth?.split('T')[0] ?? '' };
    this.editing.set(true);
    this.editId = c.candidateId;
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onSave(): void {
    if (!this.form.fullName || !this.form.email) {
      this.toast.error('Please fill in all required fields.');
      return;
    }
    this.saving.set(true);
    const payload = { ...this.form, isActive: true };

    const obs = this.editing()
      ? this.api.update('candidate', this.editId, payload)
      : this.api.create('candidate', payload);

    obs.subscribe({
      next: () => {
        this.toast.success(this.editing() ? 'Candidate updated.' : 'Candidate created.');
        this.closeModal();
        this.saving.set(false);
        this.loadData();
      },
      error: () => this.saving.set(false),
    });
  }

  onDelete(c: Candidate): void {
    if (!confirm(`Delete ${c.fullName}?`)) return;
    this.api.delete('candidate', c.candidateId).subscribe({
      next: () => { this.toast.success('Candidate deleted.'); this.loadData(); },
    });
  }

  private emptyForm(): any {
    return {
      fullName: '', email: '', phone: '',
      dateOfBirth: '', gender: '', address: '', city: '',
      country: '', linkedInUrl: '', resumeUrl: '',
    };
  }
}
