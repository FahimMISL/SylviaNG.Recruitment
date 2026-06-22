import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastService } from '../../core/services/toast.service';
import { environment } from '../../../environments/environment';

interface PipelineStage {
  hiringPipelineStageId?: number;
  jobPostingId: number;
  stageName: string;
  stageType: string;
  stageOrder: number;
  isMandatory: boolean;
  description: string;
  isActive: boolean;
  isNew?: boolean;
}

const STAGE_PRESETS: Record<string, { stageName: string; stageType: string }[]> = {
  'Software Engineer': [
    { stageName: 'CV Screening', stageType: 'Screening' },
    { stageName: 'Coding Test', stageType: 'Assessment' },
    { stageName: 'Technical Interview', stageType: 'Interview' },
    { stageName: 'HR Interview', stageType: 'Interview' },
  ],
  'Bank Job': [
    { stageName: 'CV Screening', stageType: 'Screening' },
    { stageName: 'Written Exam', stageType: 'Assessment' },
    { stageName: 'Viva', stageType: 'Interview' },
  ],
  'Simple Job': [
    { stageName: 'CV Screening', stageType: 'Screening' },
    { stageName: 'Direct Interview', stageType: 'Interview' },
  ],
};

@Component({
  selector: 'app-pipeline-builder',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <a routerLink="/job-postings">Job Postings</a>
      <span class="separator">/</span>
      <span>Hiring Pipeline</span>
    </div>

    <div class="page-header">
      <div>
        <h1 class="page-title">Hiring Pipeline Builder</h1>
        <p class="page-subtitle">{{ jobTitle() || 'Loading...' }} — Define stages for this job's hiring process</p>
      </div>
      <div style="display: flex; gap: var(--space-2);">
        <select class="form-control" style="width: auto;" (change)="applyPreset($event)">
          <option value="">Load preset...</option>
          <option value="Software Engineer">Software Engineer</option>
          <option value="Bank Job">Bank Job</option>
          <option value="Simple Job">Simple Job</option>
        </select>
        <button class="btn btn-primary" (click)="addStage()">+ Add Stage</button>
      </div>
    </div>

    @if (loading()) {
      <div class="card" style="padding: var(--space-8); text-align: center;">
        <span class="spinner"></span> Loading pipeline...
      </div>
    } @else {
      <div class="card" style="padding: var(--space-4);">
        @if (!stages().length) {
          <div class="empty-state">
            <div class="empty-state-title">No stages defined</div>
            <div class="empty-state-text">Add stages manually or load a preset template above.</div>
          </div>
        } @else {
          <div class="pipeline-stages">
            @for (stage of stages(); track stage.stageOrder; let i = $index) {
              <div class="pipeline-stage-card" [class.inactive]="!stage.isActive"
                   style="display: flex; align-items: center; gap: var(--space-3); padding: var(--space-3); border: 1px solid var(--color-border); border-radius: var(--radius-md); margin-bottom: var(--space-2);">
                <div style="font-size: var(--fs-lg); font-weight: 700; color: var(--color-text-muted); min-width: 32px; text-align: center;">
                  {{ stage.stageOrder }}
                </div>

                <div style="flex: 1; display: grid; grid-template-columns: 1fr 1fr; gap: var(--space-2);">
                  <input class="form-control" [(ngModel)]="stage.stageName" placeholder="Stage name" [attr.name]="'name_'+i" />
                  <select class="form-control" [(ngModel)]="stage.stageType" [attr.name]="'type_'+i">
                    <option value="Screening">Screening</option>
                    <option value="Assessment">Assessment</option>
                    <option value="Interview">Interview</option>
                    <option value="Background Check">Background Check</option>
                    <option value="Offer">Offer</option>
                  </select>
                </div>

                <label style="display: flex; align-items: center; gap: 4px; font-size: var(--fs-xs); white-space: nowrap; cursor: pointer;">
                  <input type="checkbox" [(ngModel)]="stage.isMandatory" [attr.name]="'mand_'+i" />
                  Required
                </label>

                <div style="display: flex; gap: 4px;">
                  <button class="btn btn-ghost btn-sm" (click)="moveUp(i)" [disabled]="i === 0" title="Move up">&#9650;</button>
                  <button class="btn btn-ghost btn-sm" (click)="moveDown(i)" [disabled]="i === stages().length - 1" title="Move down">&#9660;</button>
                  <button class="btn btn-ghost btn-sm text-danger" (click)="removeStage(i)" title="Remove">&#10005;</button>
                </div>
              </div>

              @if (i < stages().length - 1) {
                <div style="text-align: center; color: var(--color-text-muted); font-size: var(--fs-xs); margin: var(--space-1) 0;">
                  &#8595;
                </div>
              }
            }
          </div>
        }

        <div style="margin-top: var(--space-4); display: flex; justify-content: flex-end; gap: var(--space-2);">
          <a routerLink="/job-postings" class="btn btn-outline">Cancel</a>
          <button class="btn btn-primary" (click)="savePipeline()" [disabled]="saving()">
            {{ saving() ? 'Saving...' : 'Save Pipeline' }}
          </button>
        </div>
      </div>
    }
  `,
})
export class PipelineBuilderComponent implements OnInit {
  loading = signal(true);
  saving = signal(false);
  stages = signal<PipelineStage[]>([]);
  jobTitle = signal('');
  jobPostingId = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.jobPostingId = Number(this.route.snapshot.paramMap.get('jobPostingId'));
    if (!this.jobPostingId) { this.router.navigate(['/job-postings']); return; }
    this.loadJobAndPipeline();
  }

  private loadJobAndPipeline(): void {
    this.http.get<any>(`${environment.apiUrl}/job-posting/${this.jobPostingId}`).subscribe({
      next: (res) => this.jobTitle.set(res?.content?.title || res?.title || 'Job'),
      error: () => this.jobTitle.set('Unknown Job'),
    });

    this.http.get<any>(`${environment.apiUrl}/hiring-pipeline-stage/by-job/${this.jobPostingId}`).subscribe({
      next: (res) => {
        const data = res?.content ?? res ?? [];
        const stageList = Array.isArray(data) ? data : [];
        this.stages.set(stageList.sort((a: PipelineStage, b: PipelineStage) => a.stageOrder - b.stageOrder));
        this.loading.set(false);
      },
      error: () => { this.stages.set([]); this.loading.set(false); },
    });
  }

  addStage(): void {
    const current = this.stages();
    const next: PipelineStage = {
      jobPostingId: this.jobPostingId,
      stageName: '',
      stageType: 'Screening',
      stageOrder: current.length + 1,
      isMandatory: true,
      description: '',
      isActive: true,
      isNew: true,
    };
    this.stages.set([...current, next]);
  }

  removeStage(index: number): void {
    const updated = this.stages().filter((_, i) => i !== index);
    updated.forEach((s, i) => s.stageOrder = i + 1);
    this.stages.set([...updated]);
  }

  moveUp(index: number): void {
    if (index === 0) return;
    const arr = [...this.stages()];
    [arr[index - 1], arr[index]] = [arr[index], arr[index - 1]];
    arr.forEach((s, i) => s.stageOrder = i + 1);
    this.stages.set(arr);
  }

  moveDown(index: number): void {
    const arr = [...this.stages()];
    if (index >= arr.length - 1) return;
    [arr[index], arr[index + 1]] = [arr[index + 1], arr[index]];
    arr.forEach((s, i) => s.stageOrder = i + 1);
    this.stages.set(arr);
  }

  applyPreset(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    if (!value || !STAGE_PRESETS[value]) return;

    const preset = STAGE_PRESETS[value].map((s, i) => ({
      jobPostingId: this.jobPostingId,
      stageName: s.stageName,
      stageType: s.stageType,
      stageOrder: i + 1,
      isMandatory: true,
      description: '',
      isActive: true,
      isNew: true,
    }));
    this.stages.set(preset);
    this.toast.success(`Loaded "${value}" preset with ${preset.length} stages.`);
  }

  savePipeline(): void {
    const stgs = this.stages();
    if (stgs.some(s => !s.stageName.trim())) {
      this.toast.error('All stages must have a name.');
      return;
    }

    this.saving.set(true);
    let completed = 0;
    const total = stgs.length;

    if (total === 0) {
      this.saving.set(false);
      this.toast.success('Pipeline saved (empty).');
      return;
    }

    for (const stage of stgs) {
      const body = {
        jobPostingId: stage.jobPostingId,
        stageName: stage.stageName,
        stageType: stage.stageType,
        stageOrder: stage.stageOrder,
        isMandatory: stage.isMandatory,
        description: stage.description,
        isActive: stage.isActive,
      };

      const obs = stage.hiringPipelineStageId
        ? this.http.put(`${environment.apiUrl}/hiring-pipeline-stage/${stage.hiringPipelineStageId}`, body)
        : this.http.post(`${environment.apiUrl}/hiring-pipeline-stage`, body);

      obs.subscribe({
        next: () => {
          completed++;
          if (completed === total) {
            this.saving.set(false);
            this.toast.success(`Pipeline saved with ${total} stages.`);
            this.loadJobAndPipeline();
          }
        },
        error: () => {
          completed++;
          if (completed === total) { this.saving.set(false); this.toast.error('Some stages failed to save.'); }
        },
      });
    }
  }
}
