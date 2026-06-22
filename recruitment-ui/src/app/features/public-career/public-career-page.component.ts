import { Component, OnInit, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { environment } from '../../../environments/environment';

interface PublicJob {
  jobPostingId: number;
  title: string;
  description: string;
  location: string;
  employmentType: string;
  minSalary: number | null;
  maxSalary: number | null;
  postingDate: string;
  closingDate: string;
  numberOfPositions: number;
  minAge: number | null;
  maxAge: number | null;
  minExperienceYears: number | null;
  minEducationLevel: string | null;
  requiredDistrict: string | null;
}

interface CareerContent {
  careerContentId: number;
  contentType: string;
  title: string;
  body: string;
  mediaUrl: string;
  sortOrder: number;
}

@Component({
  selector: 'app-public-career-page',
  standalone: true,
  imports: [FormsModule, DatePipe, DecimalPipe],
  template: `
    <div class="public-career">
      <!-- Header -->
      <header class="career-header">
        <div class="header-content">
          <h1>Millennium Information Solution Ltd.</h1>
          <p>Smart Recruitment Platform</p>
        </div>
        <div class="header-actions">
          <button class="btn-login" (click)="goToLogin()">Login / Register</button>
        </div>
      </header>

      <!-- Hero Banner -->
      <section class="hero">
        @if (banners().length) {
          <div class="hero-banner">
            <h2>{{ banners()[0].title }}</h2>
            <p>{{ banners()[0].body }}</p>
          </div>
        } @else {
          <div class="hero-banner">
            <h2>Build Your Career With Us</h2>
            <p>Explore exciting opportunities and join our talented team. We are looking for passionate people to make a difference.</p>
          </div>
        }
        <div class="hero-search">
          <input type="text" [(ngModel)]="searchText" placeholder="Search for jobs..." (keyup.enter)="searchJobs()" />
          <button (click)="searchJobs()">Search</button>
        </div>
      </section>

      <!-- Job Listings -->
      <section class="jobs-section">
        <h2 class="section-title">Open Positions</h2>
        @if (loadingJobs()) {
          <div class="loading-state">Loading jobs...</div>
        } @else if (!jobs().length) {
          <div class="empty-state">No open positions at the moment. Check back soon!</div>
        } @else {
          <div class="jobs-grid">
            @for (job of jobs(); track job.jobPostingId) {
              <div class="job-card" (click)="selectJob(job)">
                <div class="job-card-header">
                  <h3>{{ job.title }}</h3>
                  <span class="employment-badge">{{ job.employmentType }}</span>
                </div>
                <div class="job-card-body">
                  @if (job.location) {
                    <div class="job-meta"><span class="meta-icon">&#128205;</span> {{ job.location }}</div>
                  }
                  @if (job.minSalary || job.maxSalary) {
                    <div class="job-meta"><span class="meta-icon">&#128176;</span>
                      {{ job.minSalary ? (job.minSalary | number) : '—' }} - {{ job.maxSalary ? (job.maxSalary | number) : '—' }} BDT
                    </div>
                  }
                  @if (job.numberOfPositions) {
                    <div class="job-meta"><span class="meta-icon">&#128101;</span> {{ job.numberOfPositions }} position(s)</div>
                  }
                  @if (job.closingDate) {
                    <div class="job-meta"><span class="meta-icon">&#128197;</span> Deadline: {{ job.closingDate | date:'mediumDate' }}</div>
                  }
                </div>
                <div class="job-card-footer">
                  <button class="btn-apply" (click)="goToLogin(); $event.stopPropagation()">Apply Now</button>
                </div>
              </div>
            }
          </div>

          @if (totalJobs() > jobs().length) {
            <div class="pagination">
              <button [disabled]="currentPage() <= 1" (click)="changePage(currentPage() - 1)">&larr; Previous</button>
              <span>Page {{ currentPage() }} of {{ totalPages() }}</span>
              <button [disabled]="currentPage() >= totalPages()" (click)="changePage(currentPage() + 1)">Next &rarr;</button>
            </div>
          }
        }
      </section>

      <!-- Job Detail Modal -->
      @if (selectedJob()) {
        <div class="modal-overlay" (click)="selectedJob.set(null)">
          <div class="modal-content" (click)="$event.stopPropagation()">
            <button class="modal-close" (click)="selectedJob.set(null)">&times;</button>
            <h2>{{ selectedJob()!.title }}</h2>
            <div class="detail-badges">
              <span class="employment-badge">{{ selectedJob()!.employmentType }}</span>
              @if (selectedJob()!.location) { <span class="location-badge">{{ selectedJob()!.location }}</span> }
            </div>
            @if (selectedJob()!.description) {
              <div class="detail-section">
                <h4>Description</h4>
                <p>{{ selectedJob()!.description }}</p>
              </div>
            }
            <div class="detail-grid">
              @if (selectedJob()!.minSalary || selectedJob()!.maxSalary) {
                <div class="detail-item"><span class="detail-label">Salary Range</span>
                  <span>{{ selectedJob()!.minSalary ?? '—' }} - {{ selectedJob()!.maxSalary ?? '—' }} BDT</span></div>
              }
              @if (selectedJob()!.numberOfPositions) {
                <div class="detail-item"><span class="detail-label">Positions</span><span>{{ selectedJob()!.numberOfPositions }}</span></div>
              }
              @if (selectedJob()!.closingDate) {
                <div class="detail-item"><span class="detail-label">Deadline</span><span>{{ selectedJob()!.closingDate | date:'mediumDate' }}</span></div>
              }
              @if (selectedJob()!.minAge || selectedJob()!.maxAge) {
                <div class="detail-item"><span class="detail-label">Age</span><span>{{ selectedJob()!.minAge ?? '—' }} - {{ selectedJob()!.maxAge ?? '—' }} years</span></div>
              }
              @if (selectedJob()!.minExperienceYears) {
                <div class="detail-item"><span class="detail-label">Experience</span><span>{{ selectedJob()!.minExperienceYears }}+ years</span></div>
              }
              @if (selectedJob()!.minEducationLevel) {
                <div class="detail-item"><span class="detail-label">Education</span><span>{{ selectedJob()!.minEducationLevel }}+</span></div>
              }
              @if (selectedJob()!.requiredDistrict) {
                <div class="detail-item"><span class="detail-label">District</span><span>{{ selectedJob()!.requiredDistrict }}</span></div>
              }
            </div>
            <div class="detail-actions">
              <button class="btn-apply-large" (click)="goToLogin()">Login to Apply</button>
            </div>
          </div>
        </div>
      }

      <!-- Testimonials -->
      @if (testimonials().length) {
        <section class="testimonials-section">
          <h2 class="section-title">What Our Team Says</h2>
          <div class="testimonials-grid">
            @for (t of testimonials(); track t.careerContentId) {
              <div class="testimonial-card">
                <p class="testimonial-body">"{{ t.body }}"</p>
                <p class="testimonial-author">— {{ t.title }}</p>
              </div>
            }
          </div>
        </section>
      }

      <!-- Footer -->
      <footer class="career-footer">
        <p>&copy; 2026 Millennium Information Solution Ltd. All rights reserved.</p>
        <p>Smart Recruitment Platform</p>
      </footer>
    </div>
  `,
  styles: [`
    .public-career { font-family: 'Segoe UI', Arial, sans-serif; color: #1a1a2e; }
    .career-header { display: flex; justify-content: space-between; align-items: center; padding: 16px 32px; background: #0a1628; color: white; }
    .header-content h1 { margin: 0; font-size: 20px; font-weight: 700; }
    .header-content p { margin: 4px 0 0; font-size: 12px; opacity: 0.7; }
    .btn-login { background: #c8102e; color: white; border: none; padding: 10px 24px; border-radius: 6px; font-weight: 600; cursor: pointer; }
    .btn-login:hover { background: #a50d24; }
    .hero { background: linear-gradient(135deg, #0a1628 0%, #1a2a4a 100%); color: white; padding: 64px 32px; text-align: center; }
    .hero-banner h2 { font-size: 32px; margin: 0 0 12px; }
    .hero-banner p { font-size: 16px; opacity: 0.8; max-width: 600px; margin: 0 auto 32px; }
    .hero-search { display: flex; gap: 8px; justify-content: center; max-width: 500px; margin: 0 auto; }
    .hero-search input { flex: 1; padding: 12px 16px; border: none; border-radius: 6px; font-size: 15px; }
    .hero-search button { padding: 12px 24px; background: #c8102e; color: white; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; }
    .jobs-section { padding: 48px 32px; max-width: 1200px; margin: 0 auto; }
    .section-title { font-size: 24px; margin: 0 0 24px; text-align: center; }
    .loading-state, .empty-state { text-align: center; padding: 48px; color: #888; }
    .jobs-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 20px; }
    .job-card { background: white; border: 1px solid #e8e8e8; border-radius: 10px; padding: 20px; cursor: pointer; transition: box-shadow 0.2s, transform 0.2s; }
    .job-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,0.1); transform: translateY(-2px); }
    .job-card-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 12px; }
    .job-card-header h3 { margin: 0; font-size: 17px; flex: 1; }
    .employment-badge { background: #e8f4fd; color: #0066cc; padding: 4px 10px; border-radius: 4px; font-size: 12px; font-weight: 600; white-space: nowrap; }
    .location-badge { background: #f0f0f0; color: #555; padding: 4px 10px; border-radius: 4px; font-size: 12px; }
    .job-meta { font-size: 13px; color: #666; margin-bottom: 6px; display: flex; align-items: center; gap: 6px; }
    .meta-icon { font-size: 14px; }
    .job-card-footer { margin-top: 16px; text-align: right; }
    .btn-apply { background: #c8102e; color: white; border: none; padding: 8px 20px; border-radius: 6px; font-weight: 600; font-size: 13px; cursor: pointer; }
    .btn-apply:hover { background: #a50d24; }
    .pagination { display: flex; justify-content: center; align-items: center; gap: 16px; margin-top: 32px; }
    .pagination button { padding: 8px 16px; border: 1px solid #ddd; background: white; border-radius: 6px; cursor: pointer; }
    .pagination button:disabled { opacity: 0.5; cursor: default; }
    .modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 1000; padding: 20px; }
    .modal-content { background: white; border-radius: 12px; padding: 32px; max-width: 600px; width: 100%; max-height: 80vh; overflow-y: auto; position: relative; }
    .modal-close { position: absolute; top: 12px; right: 16px; background: none; border: none; font-size: 24px; cursor: pointer; color: #999; }
    .modal-content h2 { margin: 0 0 12px; }
    .detail-badges { display: flex; gap: 8px; margin-bottom: 20px; flex-wrap: wrap; }
    .detail-section { margin-bottom: 20px; }
    .detail-section h4 { margin: 0 0 8px; color: #555; }
    .detail-section p { margin: 0; line-height: 1.6; color: #333; }
    .detail-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; margin-bottom: 24px; }
    .detail-item { background: #f8f9fa; padding: 12px; border-radius: 6px; }
    .detail-label { display: block; font-size: 12px; color: #888; margin-bottom: 4px; }
    .btn-apply-large { width: 100%; padding: 14px; background: #c8102e; color: white; border: none; border-radius: 8px; font-weight: 700; font-size: 16px; cursor: pointer; }
    .btn-apply-large:hover { background: #a50d24; }
    .detail-actions { margin-top: 8px; }
    .testimonials-section { background: #f8f9fa; padding: 48px 32px; }
    .testimonials-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 20px; max-width: 1200px; margin: 0 auto; }
    .testimonial-card { background: white; border-radius: 10px; padding: 24px; box-shadow: 0 1px 4px rgba(0,0,0,0.06); }
    .testimonial-body { font-style: italic; line-height: 1.6; color: #555; margin: 0 0 12px; }
    .testimonial-author { font-weight: 600; color: #333; margin: 0; }
    .career-footer { background: #0a1628; color: rgba(255,255,255,0.7); text-align: center; padding: 24px; }
    .career-footer p { margin: 4px 0; font-size: 13px; }
  `],
})
export class PublicCareerPageComponent implements OnInit {
  private apiUrl = environment.apiUrl;

  jobs = signal<PublicJob[]>([]);
  totalJobs = signal(0);
  currentPage = signal(1);
  totalPages = signal(1);
  loadingJobs = signal(true);
  selectedJob = signal<PublicJob | null>(null);
  searchText = '';

  banners = signal<CareerContent[]>([]);
  testimonials = signal<CareerContent[]>([]);

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    this.loadJobs();
    this.loadContent();
  }

  loadJobs(): void {
    this.loadingJobs.set(true);
    let params = new HttpParams()
      .set('page', this.currentPage().toString())
      .set('pageSize', '12');
    if (this.searchText.trim()) params = params.set('search', this.searchText.trim());

    this.http.get<any>(`${this.apiUrl}/public-career/jobs`, { params }).subscribe({
      next: (res) => {
        const data = res.content ?? res;
        this.jobs.set(data.items ?? []);
        this.totalJobs.set(data.totalCount ?? 0);
        this.totalPages.set(Math.ceil((data.totalCount ?? 0) / 12) || 1);
        this.loadingJobs.set(false);
      },
      error: () => { this.loadingJobs.set(false); },
    });
  }

  loadContent(): void {
    this.http.get<any>(`${this.apiUrl}/public-career/content`).subscribe({
      next: (res) => {
        const items: CareerContent[] = res.content ?? res ?? [];
        this.banners.set(items.filter(c => c.contentType === 'Banner'));
        this.testimonials.set(items.filter(c => c.contentType === 'Testimonial'));
      },
      error: () => {},
    });
  }

  searchJobs(): void {
    this.currentPage.set(1);
    this.loadJobs();
  }

  changePage(page: number): void {
    this.currentPage.set(page);
    this.loadJobs();
  }

  selectJob(job: PublicJob): void {
    this.selectedJob.set(job);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
