import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ToastService } from '../../core/services/toast.service';
import { ApiService } from '../../core/services/api.service';

interface JobPostingOption {
  jobPostingId: number;
  title: string;
  status: string;
  employmentType: string;
}

interface CandidateRank {
  candidateId: number;
  fullName: string;
  email: string | null;
  matchScore: number;
  explanation: string;
  matchedSkills: string[];
  missingSkills: string[];
  experienceYears: number | null;
  currentDesignation: string | null;
  profileCompleteness: number;
  shortlisted?: boolean;
  jobApplicationId?: number | null;
}

@Component({
  selector: 'app-shortlisting',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>Shortlisting</span>
    </div>

    <div class="page-header">
      <div>
        <h1 class="page-title">AI-Powered Candidate Shortlisting</h1>
        <p class="page-subtitle">AI suggests candidates ranked by fit &mdash; HR makes the final decision</p>
      </div>
    </div>

    <!-- Step 1: Job Selection -->
    <div class="card selection-card">
      <div class="card-header">
        <span class="step-badge">1</span>
        <h3>Select a Job Posting</h3>
      </div>
      <div class="card-body">
        <div class="selection-row">
          <select
            class="form-control job-select"
            [(ngModel)]="selectedJobId"
            [disabled]="loadingJobs()">
            <option [ngValue]="null">-- Select a job posting --</option>
            @for (job of jobPostings(); track job.jobPostingId) {
              <option [ngValue]="job.jobPostingId">
                {{ job.title }} ({{ job.employmentType }})
              </option>
            }
          </select>
          <button
            class="btn btn-primary analyze-btn"
            (click)="analyzeClick()"
            [disabled]="!selectedJobId || analyzing()">
            {{ analyzing() ? 'Analyzing...' : 'Analyze Candidates' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Disclaimer -->
    @if (results().length > 0) {
      <div class="disclaimer">
        <span class="disclaimer-icon">&#9888;</span>
        These rankings are AI-generated suggestions. HR should review each candidate before making final decisions.
      </div>
    }

    <!-- Results summary -->
    @if (results().length > 0) {
      <div class="results-summary">
        <span>{{ results().length }} candidates ranked</span>
        <span class="dot">&#8226;</span>
        <span class="green-text">{{ countByTier('green') }} strong</span>
        <span class="dot">&#8226;</span>
        <span class="yellow-text">{{ countByTier('yellow') }} moderate</span>
        <span class="dot">&#8226;</span>
        <span class="red-text">{{ countByTier('red') }} low</span>
      </div>
    }

    <!-- Results -->
    <div class="results-grid">
      @for (candidate of results(); track candidate.candidateId) {
        <div class="candidate-card">
          <div class="card-top">
            <div class="candidate-info">
              <h4 class="candidate-name">{{ candidate.fullName }}</h4>
              @if (candidate.email) {
                <span class="candidate-email">{{ candidate.email }}</span>
              }
              @if (candidate.currentDesignation) {
                <span class="candidate-designation">{{ candidate.currentDesignation }}</span>
              }
            </div>
            <div class="score-circle" [class]="getScoreClass(candidate.matchScore)">
              <span class="score-value">{{ candidate.matchScore }}%</span>
            </div>
          </div>

          <p class="explanation">{{ candidate.explanation }}</p>

          <!-- Skills -->
          <div class="skills-section">
            @if (candidate.matchedSkills.length > 0) {
              <div class="skill-group">
                <span class="skill-label">Matched:</span>
                @for (skill of candidate.matchedSkills; track skill) {
                  <span class="badge badge-matched">{{ skill }}</span>
                }
              </div>
            }
            @if (candidate.missingSkills.length > 0) {
              <div class="skill-group">
                <span class="skill-label">Missing:</span>
                @for (skill of candidate.missingSkills; track skill) {
                  <span class="badge badge-missing">{{ skill }}</span>
                }
              </div>
            }
          </div>

          <!-- Meta row -->
          <div class="meta-row">
            @if (candidate.experienceYears !== null) {
              <span class="meta-item">
                <strong>{{ candidate.experienceYears }}</strong> yrs exp
              </span>
            }
            <div class="completeness-bar-container">
              <span class="meta-label">Profile:</span>
              <div class="completeness-bar">
                <div
                  class="completeness-fill"
                  [style.width.%]="candidate.profileCompleteness">
                </div>
              </div>
              <span class="completeness-pct">{{ candidate.profileCompleteness }}%</span>
            </div>
          </div>

          <!-- Actions -->
          <div class="card-actions">
            <button
              class="btn btn-sm"
              [class.btn-outline]="!candidate.shortlisted"
              [class.btn-success]="candidate.shortlisted"
              [disabled]="candidate.shortlisted"
              (click)="toggleShortlist(candidate)">
              {{ candidate.shortlisted ? '&#10003; Shortlisted' : 'Shortlist' }}
            </button>
            @if (candidate.shortlisted && candidate.jobApplicationId) {
              <a class="btn btn-sm btn-link" [routerLink]="['/pipeline', candidate.jobApplicationId]">
                View Pipeline
              </a>
            }
            <a class="btn btn-sm btn-link" routerLink="/candidates">
              View Profile
            </a>
          </div>
        </div>
      }
    </div>

    <!-- Empty state -->
    @if (!analyzing() && results().length === 0 && hasAnalyzed()) {
      <div class="empty-state">
        <h3>No candidates found</h3>
        <p>There are no active candidates to rank for this job posting.</p>
      </div>
    }
  `,
  styles: [`
    .selection-card {
      background: var(--bg-primary, #fff);
      border: 1px solid var(--border-color, #e2e8f0);
      border-radius: 8px;
      margin-bottom: 1rem;
    }
    .card-header {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 1rem 1.25rem;
      border-bottom: 1px solid var(--border-color, #e2e8f0);
    }
    .card-header h3 {
      margin: 0;
      font-size: 1rem;
      font-weight: 600;
    }
    .step-badge {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 28px;
      height: 28px;
      border-radius: 50%;
      background: var(--primary, #3b82f6);
      color: #fff;
      font-weight: 700;
      font-size: 0.85rem;
    }
    .card-body {
      padding: 1.25rem;
    }
    .selection-row {
      display: flex;
      gap: 0.75rem;
      align-items: center;
    }
    .job-select {
      flex: 1;
      padding: 0.5rem 0.75rem;
      border: 1px solid var(--border-color, #d1d5db);
      border-radius: 6px;
      font-size: 0.9rem;
      background: var(--bg-primary, #fff);
      color: var(--text-primary, #111);
    }
    .analyze-btn {
      white-space: nowrap;
    }

    .disclaimer {
      display: flex;
      align-items: flex-start;
      gap: 0.5rem;
      padding: 0.75rem 1rem;
      background: #fef3c7;
      border: 1px solid #f59e0b;
      border-radius: 6px;
      font-size: 0.85rem;
      color: #92400e;
      margin-bottom: 1rem;
    }
    .disclaimer-icon {
      font-size: 1.1rem;
      line-height: 1;
    }

    .results-summary {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.5rem 0;
      font-size: 0.9rem;
      color: var(--text-secondary, #6b7280);
      margin-bottom: 0.75rem;
    }
    .dot { font-size: 0.6rem; }
    .green-text { color: #16a34a; font-weight: 600; }
    .yellow-text { color: #ca8a04; font-weight: 600; }
    .red-text { color: #dc2626; font-weight: 600; }

    .results-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(380px, 1fr));
      gap: 1rem;
    }

    .candidate-card {
      background: var(--bg-primary, #fff);
      border: 1px solid var(--border-color, #e2e8f0);
      border-radius: 8px;
      padding: 1.25rem;
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .card-top {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      gap: 1rem;
    }
    .candidate-info {
      display: flex;
      flex-direction: column;
      gap: 0.15rem;
      min-width: 0;
    }
    .candidate-name {
      margin: 0;
      font-size: 1rem;
      font-weight: 600;
      color: var(--text-primary, #111);
    }
    .candidate-email {
      font-size: 0.8rem;
      color: var(--text-secondary, #6b7280);
    }
    .candidate-designation {
      font-size: 0.8rem;
      color: var(--text-secondary, #6b7280);
      font-style: italic;
    }

    .score-circle {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 56px;
      height: 56px;
      border-radius: 50%;
      flex-shrink: 0;
    }
    .score-circle.score-green {
      background: #dcfce7;
      border: 3px solid #16a34a;
    }
    .score-circle.score-yellow {
      background: #fef9c3;
      border: 3px solid #ca8a04;
    }
    .score-circle.score-red {
      background: #fee2e2;
      border: 3px solid #dc2626;
    }
    .score-value {
      font-weight: 700;
      font-size: 0.85rem;
    }
    .score-green .score-value { color: #16a34a; }
    .score-yellow .score-value { color: #ca8a04; }
    .score-red .score-value { color: #dc2626; }

    .explanation {
      font-size: 0.85rem;
      color: var(--text-secondary, #4b5563);
      margin: 0;
      line-height: 1.4;
    }

    .skills-section {
      display: flex;
      flex-direction: column;
      gap: 0.4rem;
    }
    .skill-group {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      gap: 0.35rem;
    }
    .skill-label {
      font-size: 0.75rem;
      font-weight: 600;
      color: var(--text-secondary, #6b7280);
      margin-right: 0.15rem;
    }
    .badge {
      display: inline-block;
      padding: 0.15rem 0.5rem;
      border-radius: 12px;
      font-size: 0.72rem;
      font-weight: 500;
    }
    .badge-matched {
      background: #dcfce7;
      color: #166534;
      border: 1px solid #bbf7d0;
    }
    .badge-missing {
      background: #f3f4f6;
      color: #6b7280;
      border: 1px solid #e5e7eb;
    }

    .meta-row {
      display: flex;
      align-items: center;
      gap: 1rem;
      font-size: 0.8rem;
      color: var(--text-secondary, #6b7280);
    }
    .meta-item strong {
      color: var(--text-primary, #111);
    }
    .completeness-bar-container {
      display: flex;
      align-items: center;
      gap: 0.4rem;
      flex: 1;
    }
    .meta-label {
      font-size: 0.78rem;
      white-space: nowrap;
    }
    .completeness-bar {
      flex: 1;
      height: 6px;
      background: #e5e7eb;
      border-radius: 3px;
      overflow: hidden;
      max-width: 120px;
    }
    .completeness-fill {
      height: 100%;
      background: var(--primary, #3b82f6);
      border-radius: 3px;
      transition: width 0.3s;
    }
    .completeness-pct {
      font-size: 0.75rem;
      min-width: 30px;
    }

    .card-actions {
      display: flex;
      gap: 0.5rem;
      padding-top: 0.5rem;
      border-top: 1px solid var(--border-color, #f3f4f6);
    }
    .btn-sm {
      padding: 0.3rem 0.75rem;
      font-size: 0.8rem;
      border-radius: 5px;
    }
    .btn-outline {
      background: transparent;
      border: 1px solid var(--border-color, #d1d5db);
      color: var(--text-primary, #374151);
      cursor: pointer;
    }
    .btn-outline:hover {
      background: #f9fafb;
    }
    .btn-success {
      background: #16a34a;
      border: 1px solid #16a34a;
      color: #fff;
      cursor: pointer;
    }
    .btn-link {
      background: transparent;
      border: none;
      color: var(--primary, #3b82f6);
      cursor: pointer;
      text-decoration: none;
    }
    .btn-link:hover {
      text-decoration: underline;
    }

    .empty-state {
      text-align: center;
      padding: 3rem 1rem;
      color: var(--text-secondary, #6b7280);
    }
    .empty-state h3 {
      margin: 0 0 0.5rem;
      color: var(--text-primary, #111);
    }
  `]
})
export class ShortlistingComponent implements OnInit {
  private baseUrl = environment.apiUrl;

  jobPostings = signal<JobPostingOption[]>([]);
  loadingJobs = signal(false);
  selectedJobId: number | null = null;
  analyzing = signal(false);
  results = signal<CandidateRank[]>([]);
  hasAnalyzed = signal(false);

  constructor(
    private http: HttpClient,
    private api: ApiService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.loadJobPostings();
  }

  loadJobPostings(): void {
    this.loadingJobs.set(true);
    const params = new HttpParams()
      .set('page', '1')
      .set('pageSize', '100')
      .set('sortBy', 'title')
      .set('sortDirection', 'asc');

    this.http
      .get<any>(`${this.baseUrl}/job-posting/paged`, { params })
      .subscribe({
        next: (res) => {
          const items = res?.content?.data ?? res?.content?.items ?? res?.data ?? [];
          this.jobPostings.set(items);
          this.loadingJobs.set(false);
        },
        error: () => {
          this.toast.error('Failed to load job postings.');
          this.loadingJobs.set(false);
        }
      });
  }

  analyzeClick(): void {
    if (!this.selectedJobId) return;
    this.analyzing.set(true);
    this.hasAnalyzed.set(false);
    this.results.set([]);

    this.http
      .post<any>(`${this.baseUrl}/ai-shortlist/rank-candidates/${this.selectedJobId}`, {})
      .subscribe({
        next: (res) => {
          if (res?.hasError) {
            this.toast.error(res.decentMessage || 'Analysis failed.');
            this.analyzing.set(false);
            this.hasAnalyzed.set(true);
          } else {
            const items: CandidateRank[] = res?.content ?? [];
            if (items.length > 0) {
              this.checkExistingApplications(items);
              this.toast.success(`Ranked ${items.length} candidates.`);
            } else {
              this.results.set(items);
            }
            this.analyzing.set(false);
            this.hasAnalyzed.set(true);
          }
        },
        error: () => {
          this.toast.error('Failed to analyze candidates.');
          this.analyzing.set(false);
          this.hasAnalyzed.set(true);
        }
      });
  }

  private checkExistingApplications(items: CandidateRank[]): void {
    const params = new HttpParams()
      .set('page', '1')
      .set('pageSize', '100')
      .set('sortDirection', 'asc');

    this.http.get<any>(`${this.baseUrl}/job-application/paged`, { params }).subscribe({
      next: (res) => {
        const apps = res?.content?.data ?? res?.data ?? [];
        const appMap = new Map<number, any>();
        for (const app of apps) {
          if (app.jobPostingId === this.selectedJobId && app.isActive) {
            appMap.set(app.candidateId, app);
          }
        }
        const enriched = items.map(c => {
          const existing = appMap.get(c.candidateId);
          if (existing) {
            const status = (existing.applicationStatus ?? '').toLowerCase();
            const isShortlisted = ['shortlisted', 'interviewscheduled', 'interviewed', 'offered', 'hired'].includes(status);
            return { ...c, shortlisted: isShortlisted, jobApplicationId: existing.jobApplicationId };
          }
          return c;
        });
        this.results.set(enriched);
      },
      error: () => {
        this.results.set(items);
      }
    });
  }

  getScoreClass(score: number): string {
    if (score > 70) return 'score-green';
    if (score >= 40) return 'score-yellow';
    return 'score-red';
  }

  countByTier(tier: string): number {
    return this.results().filter(r => {
      if (tier === 'green') return r.matchScore > 70;
      if (tier === 'yellow') return r.matchScore >= 40 && r.matchScore <= 70;
      return r.matchScore < 40;
    }).length;
  }

  toggleShortlist(candidate: CandidateRank): void {
    if (candidate.shortlisted || !this.selectedJobId) return;

    this.http.post<any>(`${this.baseUrl}/job-application/shortlist`, {
      candidateId: candidate.candidateId,
      jobPostingId: this.selectedJobId
    }).subscribe({
      next: (res) => {
        const appId = res?.content?.jobApplicationId ?? res?.jobApplicationId ?? null;
        this.results.update(list =>
          list.map(c => c.candidateId === candidate.candidateId
            ? { ...c, shortlisted: true, jobApplicationId: appId }
            : c)
        );
        this.toast.success(`${candidate.fullName} has been shortlisted.`);
      },
      error: (err) => {
        const msg = err.error?.content?.message || err.error?.decentMessage || 'Failed to shortlist candidate.';
        this.toast.error(msg);
      }
    });
  }
}
