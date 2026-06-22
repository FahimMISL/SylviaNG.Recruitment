import { Component, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { ProfilePhotoService } from '../../core/services/profile-photo.service';
import { environment } from '../../../environments/environment';

interface CandidateProfile {
  candidateId: number;
  fullName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  nationalIdNumber: string;
  gender: string;
  address: string;
  city: string;
  country: string;
  profilePhotoUrl: string;
  currentDesignation: string;
  currentOrganization: string;
  totalExperienceYears: number | null;
  expectedSalary: string;
  profileCompletenessPercent: number;
  presentAddress: string;
  permanentAddress: string;
  linkedInUrl: string;
  gitHubUrl: string;
  portfolioUrl: string;
  fatherName: string;
  motherName: string;
  maritalStatus: string;
  religion: string;
  bloodGroup: string;
  isEmailVerified: boolean;
  signatureUrl: string;
  isActive: boolean;
}

const PROFILE_FIELDS: (keyof CandidateProfile)[] = [
  'fullName', 'email', 'phone', 'dateOfBirth', 'nationalIdNumber', 'gender',
  'presentAddress', 'city', 'country', 'expectedSalary',
];

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="page-breadcrumb">
      <a routerLink="/dashboard">Home</a>
      <span class="separator">/</span>
      <span>{{ isCandidate() ? 'My Profile' : 'Account Settings' }}</span>
    </div>

    <div class="page-header">
      <div>
        <h1 class="page-title">{{ isCandidate() ? 'My Profile' : 'Account Settings' }}</h1>
        <p class="page-subtitle">{{ isCandidate() ? 'Complete your profile to apply for jobs' : 'Manage your account information' }}</p>
      </div>
    </div>

    <!-- Profile Photo (all roles) -->
    <div class="card" style="padding: var(--space-6); margin-bottom: var(--space-4); max-width: 700px;">
      <h3 class="card-section-title">Profile Photo</h3>
      <div style="display: flex; align-items: center; gap: var(--space-5);">
        <div class="profile-photo-preview">
          @if (photoService.photoUrl()) {
            <img [src]="photoService.photoUrl()" alt="Profile" class="profile-photo-img" />
          } @else {
            <div class="profile-photo-placeholder">
              {{ getInitials() }}
            </div>
          }
        </div>
        <div>
          <p style="font-size: var(--fs-sm); color: var(--color-text-muted); margin-bottom: var(--space-3);">
            Upload a photo (JPEG, PNG, or WebP, max 2MB)
          </p>
          <div style="display: flex; gap: var(--space-2); align-items: center;">
            <label class="btn btn-outline btn-sm" style="cursor: pointer;">
              {{ uploadingPhoto() ? 'Uploading...' : 'Choose Photo' }}
              <input type="file" accept="image/jpeg,image/png,image/webp" (change)="onPhotoSelected($event)"
                style="display: none;" [disabled]="uploadingPhoto()" />
            </label>
            @if (photoService.photoUrl()) {
              <button class="btn btn-sm" style="color: var(--color-danger);" (click)="onDeletePhoto()">Remove</button>
            }
          </div>
        </div>
      </div>
    </div>

    @if (!isCandidate()) {
      <div style="max-width: 700px;">
        <div class="card" style="padding: var(--space-6);">
          <h3 class="card-section-title">Account Information</h3>
          <div class="form-grid">
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Full Name</label>
                <div class="form-control-static">{{ auth.currentUser()?.fullName ?? '—' }}</div>
              </div>
              <div class="form-group">
                <label class="form-label">Email</label>
                <div class="form-control-static">{{ auth.currentUser()?.email ?? '—' }}</div>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Username</label>
                <div class="form-control-static">{{ auth.currentUser()?.username ?? '—' }}</div>
              </div>
              <div class="form-group">
                <label class="form-label">Role</label>
                <div class="form-control-static">
                  @for (role of auth.currentUser()?.roles ?? []; track role) {
                    <span class="badge" style="margin-right: var(--space-2); padding: 2px 8px; background: var(--color-primary); color: white; border-radius: var(--radius-sm); font-size: var(--fs-xs);">{{ role }}</span>
                  }
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="card" style="padding: var(--space-6); margin-top: var(--space-4);">
          <h3 class="card-section-title">Security</h3>
          <p style="color: var(--color-text-muted); margin-bottom: var(--space-4); font-size: var(--fs-sm);">
            Update your password or profile details. You will be redirected to the identity provider and returned here after.
          </p>
          <div style="display: flex; gap: var(--space-3); flex-wrap: wrap;">
            <button class="btn btn-primary" (click)="auth.accountAction('UPDATE_PASSWORD')">
              Change Password
            </button>
            <button class="btn btn-outline" (click)="auth.accountAction('UPDATE_PROFILE')">
              Update Profile
            </button>
          </div>
        </div>
      </div>
    } @else if (loading()) {
      <div class="card" style="padding: var(--space-8); text-align: center;">
        <span class="spinner"></span> Loading profile...
      </div>
    } @else if (!profile()) {
      <div class="card" style="padding: var(--space-8); text-align: center;">
        <p style="color: var(--color-text-muted);">No candidate profile found for your account.</p>
      </div>
    } @else {
      <div class="profile-layout">
        <div class="profile-sidebar-card card">
          <div class="profile-progress-wrapper">
            <svg class="profile-progress-ring" viewBox="0 0 120 120">
              <circle class="progress-bg" cx="60" cy="60" r="52" />
              <circle class="progress-fill" cx="60" cy="60" r="52"
                [attr.stroke-dasharray]="circumference"
                [attr.stroke-dashoffset]="dashOffset()"
                [class.progress-low]="completeness() < 40"
                [class.progress-mid]="completeness() >= 40 && completeness() < 70"
                [class.progress-high]="completeness() >= 70" />
            </svg>
            <div class="progress-center-text">
              <span class="progress-percent">{{ completeness() }}%</span>
              <span class="progress-label">Complete</span>
            </div>
          </div>

          @if (completeness() < 70) {
            <div class="profile-warning">
              <strong>Profile incomplete</strong>
              <p>You need at least 70% to apply for jobs.</p>
            </div>
          } @else {
            <div class="profile-success">
              <strong>Profile ready</strong>
              <p>You can apply for jobs.</p>
            </div>
          }

          <div style="margin-top: var(--space-4); padding: var(--space-4); border: 1px dashed var(--color-border); border-radius: var(--radius-md);">
            <p style="font-weight: 600; font-size: var(--fs-sm); margin-bottom: var(--space-2);">Upload CV</p>
            <input type="file" accept=".pdf,.doc,.docx,.txt" (change)="onCvSelected($event)"
              style="font-size: var(--fs-xs); width: 100%;" />
            @if (uploading()) {
              <p style="font-size: var(--fs-xs); color: var(--color-text-muted); margin-top: var(--space-2);">
                <span class="spinner"></span> Uploading & parsing...
              </p>
            }
            @if (cvFileName()) {
              <p style="font-size: var(--fs-xs); color: var(--color-success); margin-top: var(--space-2);">
                &#10003; {{ cvFileName() }}
              </p>
            }
            @if (parsedData()) {
              <div style="margin-top: var(--space-3); padding: var(--space-3); background: var(--color-bg-secondary); border-radius: var(--radius-md);">
                <p style="font-weight: 600; font-size: var(--fs-xs); margin-bottom: var(--space-2);">Parsed from CV:</p>
                @if (parsedData()!.email) { <p style="font-size: var(--fs-xs);">Email: {{ parsedData()!.email }}</p> }
                @if (parsedData()!.phone) { <p style="font-size: var(--fs-xs);">Phone: {{ parsedData()!.phone }}</p> }
                @if (parsedData()!.linkedInUrl) { <p style="font-size: var(--fs-xs);">LinkedIn: {{ parsedData()!.linkedInUrl }}</p> }
                @if (parsedData()!.gitHubUrl) { <p style="font-size: var(--fs-xs);">GitHub: {{ parsedData()!.gitHubUrl }}</p> }
                @if (parsedData()!.skills?.length) { <p style="font-size: var(--fs-xs);">Skills: {{ parsedData()!.skills.join(', ') }}</p> }
                <button class="btn btn-primary btn-sm" style="margin-top: var(--space-2);" (click)="applyParsedToForm()">
                  Auto-fill Profile
                </button>
              </div>
            }
          </div>

          <div class="profile-missing-fields">
            <p class="profile-missing-title">Field Status</p>
            @for (f of fieldStatus(); track f.key) {
              <div class="field-status-row">
                <span class="field-status-icon" [class.filled]="f.filled">{{ f.filled ? '&#10003;' : '&#10007;' }}</span>
                <span>{{ f.label }}</span>
              </div>
            }
          </div>
        </div>

        <div class="profile-form-card card">
          <h3 class="card-section-title">Personal Information</h3>
          <form (ngSubmit)="onSave()" (input)="onFormInput()" (change)="onFormInput()" class="form-grid">
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Full Name <span class="required">*</span></label>
                <input class="form-control" [(ngModel)]="form.fullName" name="fullName" required />
              </div>
              <div class="form-group">
                <label class="form-label">Email <span class="required">*</span></label>
                <input class="form-control" type="email" [(ngModel)]="form.email" name="email" required />
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Phone</label>
                <input class="form-control" [(ngModel)]="form.phone" name="phone" placeholder="+880 1XXX-XXXXXX" />
              </div>
              <div class="form-group">
                <label class="form-label">Date of Birth</label>
                <input class="form-control" type="date" [(ngModel)]="form.dateOfBirth" name="dateOfBirth" />
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Gender</label>
                <select class="form-control" [(ngModel)]="form.gender" name="gender">
                  <option value="">Select</option>
                  <option value="Male">Male</option>
                  <option value="Female">Female</option>
                  <option value="Other">Other</option>
                </select>
              </div>
              <div class="form-group">
                <label class="form-label">National ID</label>
                <input class="form-control" [(ngModel)]="form.nationalIdNumber" name="nationalIdNumber" />
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Father's Name</label>
                <input class="form-control" [(ngModel)]="form.fatherName" name="fatherName" />
              </div>
              <div class="form-group">
                <label class="form-label">Mother's Name</label>
                <input class="form-control" [(ngModel)]="form.motherName" name="motherName" />
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Marital Status</label>
                <select class="form-control" [(ngModel)]="form.maritalStatus" name="maritalStatus">
                  <option value="">Select</option>
                  <option value="Single">Single</option>
                  <option value="Married">Married</option>
                  <option value="Divorced">Divorced</option>
                  <option value="Widowed">Widowed</option>
                </select>
              </div>
              <div class="form-group">
                <label class="form-label">Religion</label>
                <input class="form-control" [(ngModel)]="form.religion" name="religion" />
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Blood Group</label>
              <select class="form-control" [(ngModel)]="form.bloodGroup" name="bloodGroup" style="width: 50%;">
                <option value="">Select</option>
                <option value="A+">A+</option><option value="A-">A-</option>
                <option value="B+">B+</option><option value="B-">B-</option>
                <option value="AB+">AB+</option><option value="AB-">AB-</option>
                <option value="O+">O+</option><option value="O-">O-</option>
              </select>
            </div>

            <h3 class="card-section-title" style="margin-top: var(--space-6);">Address</h3>
            <div class="form-group">
              <label class="form-label">Present Address</label>
              <textarea class="form-control" [(ngModel)]="form.presentAddress" name="presentAddress" rows="2"></textarea>
            </div>
            <div class="form-group">
              <label style="display:flex;align-items:center;gap:8px;cursor:pointer;font-size:var(--fs-sm);margin-bottom:var(--space-2);">
                <input type="checkbox" [(ngModel)]="sameAddress" name="sameAddress" (change)="onSameAddressChange()" />
                Same as present address
              </label>
              <label class="form-label">Permanent Address</label>
              <textarea class="form-control" [(ngModel)]="form.permanentAddress" name="permanentAddress" rows="2" [disabled]="sameAddress"></textarea>
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

            <h3 class="card-section-title" style="margin-top: var(--space-6);">Professional Information</h3>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Current Designation</label>
                <input class="form-control" [(ngModel)]="form.currentDesignation" name="currentDesignation" />
              </div>
              <div class="form-group">
                <label class="form-label">Current Organization</label>
                <input class="form-control" [(ngModel)]="form.currentOrganization" name="currentOrganization" />
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Total Experience (years)</label>
                <input class="form-control" type="number" [(ngModel)]="form.totalExperienceYears" name="totalExperienceYears" min="0" max="50" />
              </div>
              <div class="form-group">
                <label class="form-label">Expected Salary</label>
                <input class="form-control" [(ngModel)]="form.expectedSalary" name="expectedSalary" placeholder="e.g. 80,000 BDT" />
              </div>
            </div>

            <h3 class="card-section-title" style="margin-top: var(--space-6);">Links</h3>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">LinkedIn URL</label>
                <input class="form-control" [(ngModel)]="form.linkedInUrl" name="linkedInUrl" placeholder="https://linkedin.com/in/..." />
              </div>
              <div class="form-group">
                <label class="form-label">GitHub URL</label>
                <input class="form-control" [(ngModel)]="form.gitHubUrl" name="gitHubUrl" placeholder="https://github.com/..." />
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Portfolio URL</label>
              <input class="form-control" [(ngModel)]="form.portfolioUrl" name="portfolioUrl" placeholder="https://..." />
            </div>

            <h3 class="card-section-title" style="margin-top: var(--space-6);">Email Verification</h3>
            <div style="display:flex;align-items:center;gap:var(--space-3);margin-bottom:var(--space-3);">
              @if (form.isEmailVerified) {
                <span style="color:var(--color-success);font-weight:600;display:flex;align-items:center;gap:6px;">
                  <span style="font-size:18px;">&#10003;</span> Email Verified
                </span>
              } @else {
                <span style="color:var(--color-warning);font-weight:600;">Not Verified</span>
                @if (!showOtpInput()) {
                  <button type="button" class="btn btn-outline btn-sm" (click)="sendOtp()" [disabled]="sendingOtp()">
                    {{ sendingOtp() ? 'Sending...' : 'Verify Email' }}
                  </button>
                }
              }
            </div>
            @if (showOtpInput() && !form.isEmailVerified) {
              <div style="display:flex;gap:8px;align-items:flex-end;margin-bottom:var(--space-3);">
                <div class="form-group" style="margin-bottom:0;">
                  <label class="form-label">Enter OTP Code</label>
                  <input class="form-control" [(ngModel)]="otpCode" name="otpCode" placeholder="6-digit code"
                    maxlength="6" style="width:160px;letter-spacing:4px;font-weight:700;text-align:center;" />
                </div>
                <button type="button" class="btn btn-primary btn-sm" (click)="verifyOtp()" [disabled]="verifyingOtp()">
                  {{ verifyingOtp() ? 'Verifying...' : 'Verify' }}
                </button>
                <button type="button" class="btn btn-sm" style="color:var(--color-text-muted);" (click)="sendOtp()" [disabled]="sendingOtp()">
                  Resend
                </button>
              </div>
            }

            <h3 class="card-section-title" style="margin-top: var(--space-6);">Signature</h3>
            <div style="margin-bottom:var(--space-3);">
              @if (form.signatureUrl) {
                <div style="margin-bottom:var(--space-3);">
                  <img [src]="apiBaseUrl + form.signatureUrl" alt="Signature" style="max-height:80px;border:1px solid var(--color-border);border-radius:var(--radius-md);padding:8px;background:white;" />
                </div>
              }
              <div style="display:flex;align-items:center;gap:var(--space-3);">
                <label class="btn btn-outline btn-sm" style="cursor:pointer;">
                  {{ uploadingSignature() ? 'Uploading...' : (form.signatureUrl ? 'Change Signature' : 'Upload Signature') }}
                  <input type="file" accept="image/jpeg,image/png" (change)="onSignatureSelected($event)"
                    style="display:none;" [disabled]="uploadingSignature()" />
                </label>
                <span style="font-size:var(--fs-xs);color:var(--color-text-muted);">JPEG or PNG, max 1MB</span>
              </div>
            </div>

            <h3 class="card-section-title" style="margin-top: var(--space-6);">Skills</h3>
            <div style="display:flex;flex-wrap:wrap;gap:8px;margin-bottom:var(--space-3);">
              @for (skill of skills(); track skill.candidateSkillId) {
                <span class="badge" style="padding:6px 12px;background:var(--color-primary);color:white;border-radius:var(--radius-md);display:flex;align-items:center;gap:6px;">
                  {{ skill.skillName }}
                  @if (skill.proficiencyLevel) {
                    <span style="opacity:0.7;font-size:var(--fs-xs);">({{ skill.proficiencyLevel }})</span>
                  }
                  <button type="button" (click)="removeSkill(skill.candidateSkillId)" style="background:none;border:none;color:white;cursor:pointer;font-size:16px;line-height:1;padding:0 2px;">&times;</button>
                </span>
              } @empty {
                <span style="color:var(--color-text-muted);font-size:var(--fs-sm);">No skills added yet.</span>
              }
            </div>
            <div style="display:flex;gap:8px;align-items:flex-end;">
              <div class="form-group" style="flex:2;margin-bottom:0;">
                <input class="form-control" [(ngModel)]="newSkillName" name="newSkillName" placeholder="Skill name (e.g. Angular, Python)" />
              </div>
              <div class="form-group" style="flex:1;margin-bottom:0;">
                <select class="form-control" [(ngModel)]="newSkillLevel" name="newSkillLevel">
                  <option value="">Proficiency</option>
                  <option value="Beginner">Beginner</option>
                  <option value="Intermediate">Intermediate</option>
                  <option value="Advanced">Advanced</option>
                  <option value="Expert">Expert</option>
                </select>
              </div>
              <button type="button" class="btn btn-outline btn-sm" (click)="addSkill()" style="white-space:nowrap;">+ Add</button>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn btn-primary" [disabled]="saving()">
                @if (saving()) { <span class="spinner"></span> }
                Save Profile
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `,
})
export class ProfileComponent implements OnInit {
  loading = signal(true);
  saving = signal(false);
  uploading = signal(false);
  profile = signal<CandidateProfile | null>(null);
  cvFileName = signal<string>('');
  parsedData = signal<any>(null);

  skills = signal<any[]>([]);
  newSkillName = '';
  newSkillLevel = '';
  sameAddress = false;
  form: any = {};
  formVersion = signal(0);

  readonly circumference = 2 * Math.PI * 52;
  isCandidate = computed(() => {
    const roles = this.auth.currentUser()?.roles ?? [];
    return roles.includes('Candidate') && !roles.includes('Admin') && !roles.includes('HR');
  });

  completeness = computed(() => {
    this.formVersion();
    if (!this.profile()) return 0;
    return this.calcCompleteness(this.form);
  });

  dashOffset = computed(() => {
    const pct = this.completeness();
    return this.circumference - (pct / 100) * this.circumference;
  });

  fieldStatus = computed(() => {
    this.formVersion();
    if (!this.profile()) return [];
    const labels: Record<string, string> = {
      fullName: 'Full Name', email: 'Email', phone: 'Phone',
      dateOfBirth: 'Date of Birth', nationalIdNumber: 'National ID',
      gender: 'Gender', presentAddress: 'Present Address',
      city: 'City', country: 'Country',
      currentDesignation: 'Current Designation',
      currentOrganization: 'Current Organization',
      totalExperienceYears: 'Experience (years)',
      expectedSalary: 'Expected Salary', linkedInUrl: 'LinkedIn',
    };
    return PROFILE_FIELDS.map((key) => ({
      key, label: labels[key] || key,
      filled: this.isFilled(this.form, key),
    }));
  });

  uploadingPhoto = signal(false);
  showOtpInput = signal(false);
  sendingOtp = signal(false);
  verifyingOtp = signal(false);
  uploadingSignature = signal(false);
  otpCode = '';
  apiBaseUrl = environment.apiUrl.replace('/recruitment', '');

  constructor(
    private api: ApiService,
    public auth: AuthService,
    private toast: ToastService,
    private http: HttpClient,
    public photoService: ProfilePhotoService
  ) {}

  ngOnInit(): void {
    if (this.isCandidate()) {
      this.loadProfile();
    }
  }

  private loadProfile(): void {
    this.api.get<CandidateProfile>('candidate-profile/my-profile').subscribe({
      next: (c) => {
        if (c) {
          this.profile.set(c);
          this.form = { ...c };
          if (this.form.dateOfBirth) this.form.dateOfBirth = this.form.dateOfBirth.substring(0, 10);
          if (c.presentAddress && c.permanentAddress && c.presentAddress === c.permanentAddress) {
            this.sameAddress = true;
          }
          this.loadSkills();
        }
        this.loading.set(false);
      },
      error: () => { this.loading.set(false); this.toast.error('Failed to load profile.'); },
    });
  }

  private loadSkills(): void {
    this.api.get<any[]>('candidate-profile/my-skills').subscribe({
      next: (data) => this.skills.set(data ?? []),
      error: () => {},
    });
  }

  addSkill(): void {
    const name = this.newSkillName.trim();
    if (!name) return;

    this.http.post<any>(`${environment.apiUrl}/candidate-profile/my-skills`, {
      skillName: name,
      proficiencyLevel: this.newSkillLevel || null,
    }).subscribe({
      next: () => {
        this.newSkillName = '';
        this.newSkillLevel = '';
        this.loadSkills();
        this.toast.success('Skill added.');
      },
      error: () => this.toast.error('Failed to add skill.'),
    });
  }

  removeSkill(skillId: number): void {
    this.http.delete<any>(`${environment.apiUrl}/candidate-profile/my-skills/${skillId}`).subscribe({
      next: () => {
        this.skills.update(s => s.filter(sk => sk.candidateSkillId !== skillId));
        this.toast.success('Skill removed.');
      },
      error: () => this.toast.error('Failed to remove skill.'),
    });
  }

  onCvSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const p = this.profile();
    if (!p) { this.toast.error('Profile not loaded.'); return; }

    this.uploading.set(true);
    this.parsedData.set(null);
    this.cvFileName.set('');

    const formData = new FormData();
    formData.append('file', file);

    this.http.post<any>(`${environment.apiUrl}/cv/upload/${p.candidateId}`, formData).subscribe({
      next: (res) => {
        this.uploading.set(false);
        if (res.hasError) {
          this.toast.error(res.decentMessage);
          return;
        }
        this.cvFileName.set(res.content?.fileName || file.name);
        const pd = res.content?.parsedData;
        const hasParsedFields = pd && (pd.email || pd.phone || pd.fullName || pd.linkedInUrl || pd.gitHubUrl || pd.skills?.length);
        if (hasParsedFields) {
          this.parsedData.set(pd);
          this.toast.success('CV uploaded and parsed! Review extracted data below.');
        } else {
          this.toast.success('CV uploaded successfully.');
        }
      },
      error: () => { this.uploading.set(false); this.toast.error('CV upload failed.'); },
    });
  }

  applyParsedToForm(): void {
    const pd = this.parsedData();
    if (!pd) return;
    if (pd.email && !this.form.email) this.form.email = pd.email;
    if (pd.phone && !this.form.phone) this.form.phone = pd.phone;
    if (pd.fullName && !this.form.fullName) this.form.fullName = pd.fullName;
    if (pd.presentAddress && !this.form.presentAddress) this.form.presentAddress = pd.presentAddress;
    if (pd.linkedInUrl && !this.form.linkedInUrl) this.form.linkedInUrl = pd.linkedInUrl;
    if (pd.gitHubUrl && !this.form.gitHubUrl) this.form.gitHubUrl = pd.gitHubUrl;
    if (pd.portfolioUrl && !this.form.portfolioUrl) this.form.portfolioUrl = pd.portfolioUrl;
    if (pd.currentDesignation && !this.form.currentDesignation) this.form.currentDesignation = pd.currentDesignation;
    if (pd.currentOrganization && !this.form.currentOrganization) this.form.currentOrganization = pd.currentOrganization;
    if (pd.totalExperienceYears != null && !this.form.totalExperienceYears) this.form.totalExperienceYears = pd.totalExperienceYears;

    if (pd.skills?.length) {
      const existing = new Set(this.skills().map((s: any) => s.skillName.toLowerCase()));
      const newSkills = pd.skills.filter((s: string) => !existing.has(s.toLowerCase()));
      for (const skill of newSkills) {
        this.http.post<any>(`${environment.apiUrl}/candidate-profile/my-skills`, {
          skillName: skill,
          proficiencyLevel: null,
        }).subscribe({ next: () => this.loadSkills() });
      }
    }
    this.parsedData.set(null);
    this.toast.success('Fields auto-filled from CV. Review and save.');
  }

  onFormInput(): void {
    this.formVersion.update(v => v + 1);
  }

  onSameAddressChange(): void {
    if (this.sameAddress) {
      this.form.permanentAddress = this.form.presentAddress;
    }
  }

  onSave(): void {
    const p = this.profile();
    if (!p) return;

    if (this.sameAddress) {
      this.form.permanentAddress = this.form.presentAddress;
    }

    this.saving.set(true);
    const body = {
      fullName: this.form.fullName, email: this.form.email,
      phone: this.form.phone || null, dateOfBirth: this.form.dateOfBirth || null,
      nationalIdNumber: this.form.nationalIdNumber || null, gender: this.form.gender || null,
      address: this.form.address || null, city: this.form.city || null, country: this.form.country || null,
      currentDesignation: this.form.currentDesignation || null,
      currentOrganization: this.form.currentOrganization || null,
      totalExperienceYears: this.form.totalExperienceYears ?? null,
      expectedSalary: this.form.expectedSalary || null,
      profilePhotoUrl: this.form.profilePhotoUrl || null,
      presentAddress: this.form.presentAddress || null,
      permanentAddress: this.form.permanentAddress || null,
      linkedInUrl: this.form.linkedInUrl || null,
      gitHubUrl: this.form.gitHubUrl || null,
      portfolioUrl: this.form.portfolioUrl || null,
      fatherName: this.form.fatherName || null,
      motherName: this.form.motherName || null,
      maritalStatus: this.form.maritalStatus || null,
      religion: this.form.religion || null,
      bloodGroup: this.form.bloodGroup || null,
      profileCompletenessPercent: this.calcCompleteness(this.form),
    };

    this.http.put<any>(`${environment.apiUrl}/candidate-profile/my-profile`, body).subscribe({
      next: () => {
        const updated = { ...p, ...body, profileCompletenessPercent: body.profileCompletenessPercent };
        this.profile.set(updated as CandidateProfile);
        this.saving.set(false);
        this.toast.success('Profile saved successfully!');
      },
      error: () => { this.saving.set(false); this.toast.error('Failed to save profile.'); },
    });
  }

  getInitials(): string {
    const name = this.auth.currentUser()?.fullName ?? 'U';
    return name.split(' ').map((w) => w[0]).join('').substring(0, 2).toUpperCase();
  }

  async onPhotoSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    if (file.size > 2 * 1024 * 1024) {
      this.toast.error('Photo must be under 2MB.');
      return;
    }

    this.uploadingPhoto.set(true);
    const success = await this.photoService.upload(file);
    this.uploadingPhoto.set(false);

    if (success) {
      this.toast.success('Profile photo updated!');
    } else {
      this.toast.error('Failed to upload photo.');
    }
    input.value = '';
  }

  async onDeletePhoto(): Promise<void> {
    const success = await this.photoService.deletePhoto();
    if (success) {
      this.toast.success('Profile photo removed.');
    } else {
      this.toast.error('Failed to remove photo.');
    }
  }

  sendOtp(): void {
    this.sendingOtp.set(true);
    this.http.post<any>(`${environment.apiUrl}/candidate-profile/send-otp`, {}).subscribe({
      next: (res) => {
        this.sendingOtp.set(false);
        const data = res.content ?? res;
        if (data.success) {
          this.showOtpInput.set(true);
          this.toast.success(data.message);
        } else {
          this.toast.error(data.message);
        }
      },
      error: () => { this.sendingOtp.set(false); this.toast.error('Failed to send OTP.'); },
    });
  }

  verifyOtp(): void {
    if (!this.otpCode.trim()) { this.toast.error('Please enter the OTP code.'); return; }
    this.verifyingOtp.set(true);
    this.http.post<any>(`${environment.apiUrl}/candidate-profile/verify-otp`, { otpCode: this.otpCode.trim() }).subscribe({
      next: (res) => {
        this.verifyingOtp.set(false);
        const data = res.content ?? res;
        if (data.success) {
          this.form.isEmailVerified = true;
          this.showOtpInput.set(false);
          this.otpCode = '';
          this.toast.success('Email verified successfully!');
        } else {
          this.toast.error(data.message);
        }
      },
      error: () => { this.verifyingOtp.set(false); this.toast.error('Verification failed.'); },
    });
  }

  onSignatureSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    if (file.size > 1024 * 1024) { this.toast.error('Signature must be under 1MB.'); return; }

    this.uploadingSignature.set(true);
    const formData = new FormData();
    formData.append('file', file);

    this.http.post<any>(`${environment.apiUrl}/candidate-profile/upload-signature`, formData).subscribe({
      next: (res) => {
        this.uploadingSignature.set(false);
        const data = res.content ?? res;
        this.form.signatureUrl = data.signatureUrl;
        this.toast.success('Signature uploaded!');
      },
      error: () => { this.uploadingSignature.set(false); this.toast.error('Signature upload failed.'); },
    });
    input.value = '';
  }

  private calcCompleteness(obj: any): number {
    let filled = 0;
    for (const key of PROFILE_FIELDS) { if (this.isFilled(obj, key)) filled++; }
    return Math.round((filled / PROFILE_FIELDS.length) * 100);
  }

  private isFilled(obj: any, key: string): boolean {
    const val = obj[key];
    if (val === null || val === undefined || val === '') return false;
    if (typeof val === 'string' && val.trim() === '') return false;
    return true;
  }
}
