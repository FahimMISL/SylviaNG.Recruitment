import { Component, OnInit, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environments/environment';
import { ToastService } from '../../core/services/toast.service';

interface StageProgress {
  hiringPipelineStageId: number;
  stageName: string;
  stageType: string;
  stageOrder: number;
  isMandatory: boolean;
  status: string;
  notes: string | null;
  meetingLink: string | null;
  scheduledDate: string | null;
  completedAt: string | null;
  candidateStageProgressId: number | null;
}

interface PipelineStatus {
  jobApplicationId: number;
  candidateId: number;
  candidateName: string;
  candidateEmail: string;
  jobTitle: string;
  applicationStatus: string;
  pipelineStages: StageProgress[];
}

@Component({
  selector: 'app-pipeline-tracker',
  standalone: true,
  imports: [DatePipe, FormsModule, RouterLink],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <a routerLink="/applications">Applications</a>
      <span class="separator">/</span>
      <span>Pipeline Tracker</span>
    </div>
    <div class="page-header">
      <div>
        <h1 class="page-title">Recruitment Pipeline</h1>
        <p class="page-subtitle">Track and advance candidate through hiring stages</p>
      </div>
    </div>

    @if (loading()) {
      <div class="loading-state">Loading pipeline status...</div>
    } @else if (pipeline()) {
      <div class="candidate-card">
        <div class="candidate-info">
          <h2>{{ pipeline()!.candidateName }}</h2>
          <p>{{ pipeline()!.candidateEmail }}</p>
          <p class="job-title">Position: <strong>{{ pipeline()!.jobTitle }}</strong></p>
        </div>
        <div class="status-badge-wrapper">
          <span class="current-status" [class]="'status-' + pipeline()!.applicationStatus.toLowerCase()">
            {{ pipeline()!.applicationStatus }}
          </span>
        </div>
      </div>

      @if (!pipeline()!.pipelineStages?.length) {
        <div class="no-stages-card">
          <p>No pipeline stages configured for this job posting.</p>
          <p class="stage-hint">HR should configure stages via the Pipeline Builder when creating the job posting.</p>
        </div>
      } @else {
        <div class="pipeline-stages">
          @for (stage of pipeline()!.pipelineStages; track stage.hiringPipelineStageId; let i = $index) {
            <div class="stage-card"
                 [class.completed]="stage.status === 'Completed'"
                 [class.failed]="stage.status === 'Failed'"
                 [class.active]="stage.status === 'InProgress' || isNextStage(i)">
              <div class="stage-header">
                <div class="stage-number">{{ stage.stageOrder }}</div>
                <h3>{{ stage.stageName }}</h3>
                <span class="stage-type-badge">{{ stage.stageType }}</span>
                @if (stage.status !== 'Pending') {
                  <span class="stage-badge" [class]="'badge-' + stage.status.toLowerCase()">
                    {{ stage.status }}
                  </span>
                }
              </div>
              <div class="stage-body">

                <!-- PENDING: show Start button if this is the next stage -->
                @if (stage.status === 'Pending' && isNextStage(i)) {

                  @if (stage.stageType === 'Interview') {
                    <p class="stage-hint">Schedule an interview for this candidate</p>
                    <div class="form-row">
                      <div class="form-group">
                        <label class="form-label">Scheduled Date *</label>
                        <input class="form-control" type="datetime-local" [(ngModel)]="stageDate" />
                      </div>
                      <div class="form-group">
                        <label class="form-label">Location *</label>
                        <select class="form-control" [(ngModel)]="stageLocation">
                          <option value="Office">Office (On-site)</option>
                          <option value="Virtual">Virtual (Online)</option>
                        </select>
                      </div>
                    </div>
                    @if (stageLocation === 'Virtual') {
                      <div class="form-group">
                        <label class="form-label">Meeting Link *</label>
                        <input class="form-control" [(ngModel)]="stageMeetingLink" placeholder="https://meet.google.com/... or https://zoom.us/..." />
                        <small class="field-hint">Paste the video conference link (Zoom, Google Meet, Teams, etc.)</small>
                      </div>
                    }
                    <button class="btn btn-primary" [disabled]="actionLoading()" (click)="startStage(stage)">
                      {{ actionLoading() ? 'Scheduling...' : 'Schedule Interview' }}
                    </button>
                  } @else if (stage.stageType === 'Assessment') {
                    <p class="stage-hint">Start the assessment/exam for this candidate</p>
                    <div class="form-group">
                      <label class="form-label">Notes (optional)</label>
                      <textarea class="form-control" [(ngModel)]="stageNotes" rows="2" placeholder="Assessment details..."></textarea>
                    </div>
                    <button class="btn btn-primary" [disabled]="actionLoading()" (click)="startStage(stage)">
                      {{ actionLoading() ? 'Starting...' : 'Start Assessment' }}
                    </button>
                  } @else if (stage.stageType === 'Background Check') {
                    <p class="stage-hint">Initiate background verification for this candidate</p>
                    <button class="btn btn-primary" [disabled]="actionLoading()" (click)="startStage(stage)">
                      {{ actionLoading() ? 'Starting...' : 'Start Background Check' }}
                    </button>
                  } @else if (stage.stageType === 'Offer') {
                    <p class="stage-hint">Extend an offer to this candidate</p>
                    <div class="form-group">
                      <label class="form-label">Notes (optional)</label>
                      <textarea class="form-control" [(ngModel)]="stageNotes" rows="2" placeholder="Offer details..."></textarea>
                    </div>
                    <button class="btn btn-primary" [disabled]="actionLoading()" (click)="startStage(stage)">
                      {{ actionLoading() ? 'Processing...' : 'Start Offer Process' }}
                    </button>
                  } @else {
                    <p class="stage-hint">Start the {{ stage.stageName }} stage</p>
                    <button class="btn btn-primary" [disabled]="actionLoading()" (click)="startStage(stage)">
                      {{ actionLoading() ? 'Starting...' : 'Start ' + stage.stageName }}
                    </button>
                  }

                } @else if (stage.status === 'Pending') {
                  <p class="stage-hint-disabled">Complete the previous stage(s) first</p>

                } @else if (stage.status === 'InProgress') {
                  <!-- IN PROGRESS: show details + complete/fail buttons -->
                  <div class="stage-details">
                    @if (stage.scheduledDate) {
                      <p><strong>Scheduled:</strong> {{ stage.scheduledDate | date:'medium' }}</p>
                    }
                    @if (stage.meetingLink) {
                      <p><strong>Meeting Link:</strong> <a [href]="stage.meetingLink" target="_blank" class="meeting-link">Join Meeting</a></p>
                    }
                    @if (stage.notes) {
                      <p><strong>Notes:</strong> {{ stage.notes }}</p>
                    }
                  </div>
                  <div class="form-group">
                    <label class="form-label">Feedback / Notes</label>
                    <textarea class="form-control" [(ngModel)]="completionNotes" rows="2" placeholder="Enter notes or feedback..."></textarea>
                  </div>
                  <div class="stage-actions">
                    <button class="btn btn-success" [disabled]="actionLoading()" (click)="completeStage(stage, true)">
                      {{ actionLoading() ? 'Saving...' : 'Mark as Passed' }}
                    </button>
                    <button class="btn btn-danger" [disabled]="actionLoading()" (click)="completeStage(stage, false)">
                      {{ actionLoading() ? 'Saving...' : 'Mark as Failed' }}
                    </button>
                  </div>

                } @else if (stage.status === 'Completed') {
                  <div class="stage-details">
                    @if (stage.completedAt) {
                      <p><strong>Completed:</strong> {{ stage.completedAt | date:'medium' }}</p>
                    }
                    @if (stage.scheduledDate) {
                      <p><strong>Scheduled:</strong> {{ stage.scheduledDate | date:'medium' }}</p>
                    }
                    @if (stage.meetingLink) {
                      <p><strong>Meeting Link:</strong> <a [href]="stage.meetingLink" target="_blank" class="meeting-link">{{ stage.meetingLink }}</a></p>
                    }
                    @if (stage.notes) {
                      <p><strong>Notes:</strong> {{ stage.notes }}</p>
                    }
                  </div>

                } @else if (stage.status === 'Failed') {
                  <div class="stage-details failed-text">
                    <p><strong>Result:</strong> Failed</p>
                    @if (stage.completedAt) {
                      <p><strong>Completed:</strong> {{ stage.completedAt | date:'medium' }}</p>
                    }
                    @if (stage.notes) {
                      <p><strong>Notes:</strong> {{ stage.notes }}</p>
                    }
                  </div>
                }

              </div>
            </div>
          }
        </div>

        @if (pipeline()!.applicationStatus === 'Offered' || pipeline()!.applicationStatus === 'Hired') {
          <div class="final-section">
            @if (pipeline()!.applicationStatus === 'Hired') {
              <div class="hired-banner">Candidate has been successfully hired!</div>
            } @else {
              <div class="offered-banner">
                <p>All stages completed — Offer extended to candidate.</p>
              </div>
            }
          </div>
        }
      }
    }
  `,
  styles: [`
    .loading-state { text-align: center; padding: 3rem; color: var(--text-secondary, #6b7280); }
    .candidate-card {
      display: flex; justify-content: space-between; align-items: center;
      background: var(--bg-primary, #fff); border: 1px solid var(--border-color, #e5e7eb);
      border-radius: 8px; padding: 1.5rem; margin-bottom: 2rem;
    }
    .candidate-info h2 { margin: 0 0 4px; font-size: 1.25rem; }
    .candidate-info p { margin: 0; color: var(--text-secondary, #6b7280); font-size: 0.875rem; }
    .job-title { margin-top: 8px !important; }
    .current-status {
      padding: 0.5rem 1rem; border-radius: 6px; font-weight: 600; font-size: 0.875rem;
      background: var(--bg-secondary, #f3f4f6); color: var(--text-primary, #111827);
    }
    .status-shortlisted { background: #dbeafe; color: #1e40af; }
    .status-interviewscheduled { background: #fef3c7; color: #92400e; }
    .status-interviewed { background: #d1fae5; color: #065f46; }
    .status-offered { background: #ede9fe; color: #5b21b6; }
    .status-hired { background: #d1fae5; color: #065f46; }
    .status-rejected { background: #fee2e2; color: #991b1b; }

    .no-stages-card {
      background: var(--bg-primary, #fff); border: 2px dashed var(--border-color, #e5e7eb);
      border-radius: 8px; padding: 2rem; text-align: center;
    }
    .no-stages-card p { margin: 0.25rem 0; }

    .pipeline-stages { display: flex; flex-direction: column; gap: 1rem; }
    .stage-card {
      background: var(--bg-primary, #fff); border: 2px solid var(--border-color, #e5e7eb);
      border-radius: 8px; overflow: hidden; opacity: 0.6;
    }
    .stage-card.active, .stage-card.completed { opacity: 1; }
    .stage-card.completed { border-color: #10b981; }
    .stage-card.active { border-color: #3b82f6; }
    .stage-card.failed { border-color: #ef4444; opacity: 1; }
    .stage-header {
      display: flex; align-items: center; gap: 0.75rem; padding: 1rem 1.5rem;
      background: var(--bg-secondary, #f9fafb); border-bottom: 1px solid var(--border-color, #e5e7eb);
    }
    .stage-number {
      width: 28px; height: 28px; border-radius: 50%; display: flex; align-items: center; justify-content: center;
      background: var(--border-color, #d1d5db); color: white; font-weight: 700; font-size: 0.875rem;
    }
    .stage-card.active .stage-number { background: #3b82f6; }
    .stage-card.completed .stage-number { background: #10b981; }
    .stage-card.failed .stage-number { background: #ef4444; }
    .stage-header h3 { margin: 0; font-size: 1rem; flex: 1; }
    .stage-type-badge {
      padding: 0.15rem 0.5rem; border-radius: 4px; font-size: 0.7rem; font-weight: 500;
      background: #f3f4f6; color: #6b7280; border: 1px solid #e5e7eb;
    }
    .stage-badge { padding: 0.2rem 0.5rem; border-radius: 4px; font-size: 0.75rem; font-weight: 500; }
    .badge-pending { background: #fef3c7; color: #92400e; }
    .badge-inprogress { background: #dbeafe; color: #1e40af; }
    .badge-completed { background: #d1fae5; color: #065f46; }
    .badge-failed { background: #fee2e2; color: #991b1b; }
    .stage-body { padding: 1rem 1.5rem; }
    .stage-hint { color: var(--text-secondary, #6b7280); margin: 0 0 1rem; font-size: 0.875rem; }
    .stage-hint-disabled { color: var(--text-secondary, #9ca3af); margin: 0; font-size: 0.875rem; font-style: italic; }
    .stage-details p { margin: 0.25rem 0; font-size: 0.875rem; }
    .failed-text p { color: #991b1b; }
    .stage-actions { display: flex; gap: 0.5rem; margin-top: 0.75rem; }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 0.75rem; }
    .form-group { margin-bottom: 0.5rem; }
    .form-label { display: block; font-size: 0.75rem; font-weight: 600; margin-bottom: 4px; color: var(--text-secondary, #6b7280); }
    .form-control {
      width: 100%; padding: 0.5rem 0.75rem; font-size: 0.875rem; border: 1px solid var(--border-color, #d1d5db);
      border-radius: 6px; background: var(--bg-primary, #fff); color: var(--text-primary, #111827);
    }
    .form-control:focus { outline: none; border-color: #3b82f6; box-shadow: 0 0 0 2px rgba(59,130,246,0.15); }
    textarea.form-control { resize: vertical; }
    .btn { padding: 0.5rem 1rem; border-radius: 6px; font-size: 0.875rem; font-weight: 500; border: none; cursor: pointer; }
    .btn-primary { background: #3b82f6; color: #fff; }
    .btn-primary:hover { background: #2563eb; }
    .btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }
    .btn-success { background: #10b981; color: #fff; }
    .btn-success:hover { background: #059669; }
    .btn-danger { background: #ef4444; color: #fff; }
    .btn-danger:hover { background: #dc2626; }
    .meeting-link { color: #3b82f6; font-weight: 600; text-decoration: underline; }
    .meeting-link:hover { color: #2563eb; }
    .field-hint { display: block; margin-top: 4px; font-size: 0.75rem; color: var(--text-secondary, #6b7280); }
    .final-section { margin-top: 1.5rem; }
    .hired-banner {
      padding: 1rem; background: #d1fae5; color: #065f46;
      border-radius: 6px; font-weight: 600; text-align: center; font-size: 1rem;
    }
    .offered-banner {
      padding: 1rem; background: #ede9fe; color: #5b21b6;
      border-radius: 6px; font-weight: 600; text-align: center; font-size: 1rem;
    }
    .offered-banner p { margin: 0; }
  `]
})
export class PipelineTrackerComponent implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private toast = inject(ToastService);
  private baseUrl = environment.apiUrl;

  pipeline = signal<PipelineStatus | null>(null);
  loading = signal(false);
  actionLoading = signal(false);

  stageDate = '';
  stageLocation = 'Office';
  stageMeetingLink = '';
  stageNotes = '';
  completionNotes = '';

  private jobApplicationId = 0;

  ngOnInit(): void {
    this.jobApplicationId = Number(this.route.snapshot.paramMap.get('jobApplicationId'));
    this.loadPipeline();
  }

  loadPipeline(): void {
    this.loading.set(true);
    this.http.get<any>(`${this.baseUrl}/pipeline/status/${this.jobApplicationId}`).subscribe({
      next: (res) => {
        this.pipeline.set(res.content ?? res);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  isNextStage(index: number): boolean {
    const stages = this.pipeline()?.pipelineStages;
    if (!stages) return false;
    if (stages[index].status !== 'Pending') return false;
    if (index === 0) return true;
    return stages[index - 1].status === 'Completed';
  }

  startStage(stage: StageProgress): void {
    if (stage.stageType === 'Interview' && !this.stageDate) {
      this.toast.error('Please select a date and time.');
      return;
    }
    if (stage.stageType === 'Interview' && this.stageLocation === 'Virtual' && !this.stageMeetingLink.trim()) {
      this.toast.error('Please provide a meeting link for virtual interviews.');
      return;
    }

    this.actionLoading.set(true);
    this.http.post<any>(`${this.baseUrl}/pipeline/stage/start`, {
      jobApplicationId: this.jobApplicationId,
      hiringPipelineStageId: stage.hiringPipelineStageId,
      scheduledDate: this.stageDate ? new Date(this.stageDate).toISOString() : null,
      meetingLink: (stage.stageType === 'Interview' && this.stageLocation === 'Virtual') ? this.stageMeetingLink.trim() : null,
      notes: this.stageNotes || null
    }).subscribe({
      next: (res) => {
        this.toast.success(res.content?.message ?? res.message ?? 'Stage started.');
        this.resetForm();
        this.actionLoading.set(false);
        this.loadPipeline();
      },
      error: (err) => {
        this.toast.error(err.error?.decentMessage || err.error?.content?.message || 'Failed to start stage.');
        this.actionLoading.set(false);
      }
    });
  }

  completeStage(stage: StageProgress, passed: boolean): void {
    if (!stage.candidateStageProgressId) return;
    this.actionLoading.set(true);
    this.http.post<any>(`${this.baseUrl}/pipeline/stage/complete`, {
      candidateStageProgressId: stage.candidateStageProgressId,
      passed,
      notes: this.completionNotes || null
    }).subscribe({
      next: (res) => {
        this.toast.success(res.content?.message ?? res.message ?? `Stage marked as ${passed ? 'Passed' : 'Failed'}.`);
        this.completionNotes = '';
        this.actionLoading.set(false);
        this.loadPipeline();
      },
      error: (err) => {
        this.toast.error(err.error?.decentMessage || 'Failed to update stage.');
        this.actionLoading.set(false);
      }
    });
  }

  private resetForm(): void {
    this.stageDate = '';
    this.stageLocation = 'Office';
    this.stageMeetingLink = '';
    this.stageNotes = '';
  }
}
