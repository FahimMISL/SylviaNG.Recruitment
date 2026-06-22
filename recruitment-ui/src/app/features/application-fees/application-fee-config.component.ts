import { Component, OnInit, signal } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ToastService } from '../../core/services/toast.service';
import { SKIP_ERROR_TOAST } from '../../core/interceptors/error.interceptor';
import { ModalComponent } from '../../shared/components/modal.component';

interface ApplicationFee {
  applicationFeeId: number;
  jobPostingId: number;
  jobTitle: string;
  amount: number;
  currency: string;
  paymentMethods: string;
  waiverRules: string;
}

interface JobOption {
  jobPostingId: number;
  title: string;
}

@Component({
  selector: 'app-application-fee-config',
  standalone: true,
  imports: [FormsModule, ModalComponent],
  template: `
    <div class="page-header">
      <div>
        <h1 class="page-title">Application Fees</h1>
        <p class="page-subtitle">Configure application fees for job postings</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ Add Fee</button>
    </div>

    @if (loading()) {
      <div class="loading-state">Loading...</div>
    } @else if (!fees().length) {
      <div class="empty-state">
        <h3>No application fees configured</h3>
        <p>Add a fee to require payment before candidates can apply.</p>
      </div>
    } @else {
      <div class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>Job Posting</th>
              <th>Amount</th>
              <th>Currency</th>
              <th>Payment Methods</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (fee of fees(); track fee.applicationFeeId) {
              <tr>
                <td>{{ fee.jobTitle }}</td>
                <td>{{ fee.amount }}</td>
                <td>{{ fee.currency }}</td>
                <td>{{ fee.paymentMethods || 'All' }}</td>
                <td>
                  <div class="action-buttons">
                    <button class="action-btn action-edit" title="Edit" (click)="openEdit(fee)">&#9998;</button>
                    <button class="action-btn action-delete" title="Delete" (click)="deleteFee(fee)">&#128465;</button>
                  </div>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    }

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit Application Fee' : 'Add Application Fee'" size="md" (close)="showModal.set(false)">
      <div class="form-group">
        <label class="form-label">Job Posting</label>
        <select class="form-control" [(ngModel)]="form.jobPostingId" [disabled]="!!editing()">
          <option [ngValue]="0">Select a job...</option>
          @for (job of jobOptions(); track job.jobPostingId) {
            <option [ngValue]="job.jobPostingId">{{ job.title }}</option>
          }
        </select>
      </div>
      <div class="form-group">
        <label class="form-label">Amount</label>
        <input class="form-control" type="number" [(ngModel)]="form.amount" min="0" step="0.01" />
      </div>
      <div class="form-group">
        <label class="form-label">Currency</label>
        <input class="form-control" [(ngModel)]="form.currency" placeholder="BDT" />
      </div>
      <div class="form-group">
        <label class="form-label">Payment Methods (optional)</label>
        <input class="form-control" [(ngModel)]="form.paymentMethods" placeholder="e.g. bKash, Card, Bank" />
      </div>
      <div class="form-group">
        <label class="form-label">Waiver Rules (optional)</label>
        <textarea class="form-control" [(ngModel)]="form.waiverRules" rows="3" placeholder="e.g. Freedom fighters exempt"></textarea>
      </div>
      <div class="form-actions">
        <button class="btn btn-outline" (click)="showModal.set(false)">Cancel</button>
        <button class="btn btn-primary" [disabled]="saving()" (click)="save()">
          {{ saving() ? 'Saving...' : 'Save' }}
        </button>
      </div>
    </app-modal>
  `,
  styles: [`
    .loading-state, .empty-state { text-align: center; padding: 3rem; color: #888; }
    .table-container { overflow-x: auto; }
    .data-table { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; }
    .data-table th, .data-table td { padding: 12px 16px; text-align: left; border-bottom: 1px solid #e5e7eb; }
    .data-table th { background: #f9fafb; font-weight: 600; font-size: 0.813rem; text-transform: uppercase; color: #6b7280; }
  `],
})
export class ApplicationFeeConfigComponent implements OnInit {
  private baseUrl = environment.apiUrl;

  fees = signal<ApplicationFee[]>([]);
  jobOptions = signal<JobOption[]>([]);
  loading = signal(true);
  showModal = signal(false);
  saving = signal(false);
  editing = signal<ApplicationFee | null>(null);

  form = { jobPostingId: 0, amount: 0, currency: 'BDT', paymentMethods: '', waiverRules: '' };

  constructor(private http: HttpClient, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadFees();
  }

  loadFees(): void {
    this.http.get<any>(`${this.baseUrl}/application-fee`, {
      context: new HttpContext().set(SKIP_ERROR_TOAST, true)
    }).pipe(
      catchError(() => of(null))
    ).subscribe({
      next: (res) => {
        if (res) this.fees.set(res.content ?? res ?? []);
        this.loading.set(false);
      },
    });
  }

  loadJobs(): void {
    this.http.get<any>(`${this.baseUrl}/job-posting`, {
      context: new HttpContext().set(SKIP_ERROR_TOAST, true)
    }).pipe(
      catchError(() => of(null))
    ).subscribe({
      next: (res) => {
        if (!res) return;
        const items = res.content ?? res ?? [];
        this.jobOptions.set((Array.isArray(items) ? items : []).map((j: any) => ({ jobPostingId: j.jobPostingId, title: j.title })));
      },
    });
  }

  openCreate(): void {
    this.editing.set(null);
    this.form = { jobPostingId: 0, amount: 0, currency: 'BDT', paymentMethods: '', waiverRules: '' };
    this.loadJobs();
    this.showModal.set(true);
  }

  openEdit(fee: ApplicationFee): void {
    this.editing.set(fee);
    this.form = {
      jobPostingId: fee.jobPostingId,
      amount: fee.amount,
      currency: fee.currency,
      paymentMethods: fee.paymentMethods || '',
      waiverRules: fee.waiverRules || '',
    };
    this.showModal.set(true);
  }

  save(): void {
    if (!this.form.jobPostingId || this.form.amount <= 0) {
      this.toast.error('Please select a job and enter a valid amount.');
      return;
    }
    this.saving.set(true);

    const editing = this.editing();
    const body = { ...this.form };
    const req = editing
      ? this.http.put<any>(`${this.baseUrl}/application-fee/${editing.applicationFeeId}`, body)
      : this.http.post<any>(`${this.baseUrl}/application-fee`, body);

    req.subscribe({
      next: () => {
        this.saving.set(false);
        this.showModal.set(false);
        this.loadFees();
        this.toast.success(editing ? 'Fee updated.' : 'Fee created.');
      },
      error: () => { this.saving.set(false); this.toast.error('Failed to save.'); },
    });
  }

  deleteFee(fee: ApplicationFee): void {
    this.http.delete<any>(`${this.baseUrl}/application-fee/${fee.applicationFeeId}`).subscribe({
      next: () => {
        this.fees.update(list => list.filter(f => f.applicationFeeId !== fee.applicationFeeId));
        this.toast.success('Fee deleted.');
      },
      error: () => this.toast.error('Failed to delete.'),
    });
  }
}
