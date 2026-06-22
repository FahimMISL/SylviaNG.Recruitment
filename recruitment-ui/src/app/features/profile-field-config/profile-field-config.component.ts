import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { ModalComponent } from '../../shared/components/modal.component';

interface ProfileField {
  profileFieldConfigId: number;
  fieldName: string;
  label: string;
  fieldType: string;
  isRequired: boolean;
  isVisible: boolean;
  sortOrder: number;
  optionsJson: string | null;
  isActive: boolean;
}

@Component({
  selector: 'app-profile-field-config',
  standalone: true,
  imports: [FormsModule, ModalComponent],
  template: `
    <div class="page-header">
      <div>
        <h1 class="page-title">Profile Field Configuration</h1>
        <p class="page-subtitle">Configure which fields are visible and required on candidate profiles</p>
      </div>
      <button class="btn btn-primary" (click)="openCreate()">+ Add Field</button>
    </div>

    @if (loading()) {
      <div class="card-body">
        @for (i of [1,2,3]; track i) {
          <div class="skeleton skeleton-text" [style.width]="(90 - i * 10) + '%'"></div>
        }
      </div>
    } @else if (fields().length === 0) {
      <div class="empty-state">
        <div class="empty-state-icon">&#9881;</div>
        <div class="empty-state-title">No profile fields configured</div>
        <div class="empty-state-text">Add fields to customize the candidate profile form.</div>
      </div>
    } @else {
      <div class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>Order</th>
              <th>Field Name</th>
              <th>Label</th>
              <th>Type</th>
              <th>Required</th>
              <th>Visible</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            @for (field of fields(); track field.profileFieldConfigId) {
              <tr>
                <td>{{ field.sortOrder }}</td>
                <td><code>{{ field.fieldName }}</code></td>
                <td>{{ field.label }}</td>
                <td><span class="badge badge-neutral">{{ field.fieldType }}</span></td>
                <td>
                  <span class="badge" [class.badge-success]="field.isRequired" [class.badge-neutral]="!field.isRequired">
                    {{ field.isRequired ? 'Yes' : 'No' }}
                  </span>
                </td>
                <td>
                  <span class="badge" [class.badge-success]="field.isVisible" [class.badge-neutral]="!field.isVisible">
                    {{ field.isVisible ? 'Yes' : 'No' }}
                  </span>
                </td>
                <td>
                  <span class="badge" [class.badge-success]="field.isActive" [class.badge-danger]="!field.isActive">
                    {{ field.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td>
                  <div class="action-buttons">
                    <button class="action-btn action-edit" title="Edit" (click)="openEdit(field)">&#9998;</button>
                    <button class="action-btn action-delete" title="Delete" (click)="onDelete(field)">&#128465;</button>
                  </div>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    }

    <app-modal [open]="showModal()" [title]="editing() ? 'Edit Field' : 'Add Field'" size="md" (close)="closeModal()">
      <form (ngSubmit)="onSave()" class="form-grid">
        <div class="form-group">
          <label class="form-label">Field Name *</label>
          <input class="form-control" [(ngModel)]="form.fieldName" name="fieldName" placeholder="e.g. fathersName" required [disabled]="editing()" />
          <small style="color:#6b7280">Internal identifier. Cannot be changed after creation.</small>
        </div>
        <div class="form-group">
          <label class="form-label">Label *</label>
          <input class="form-control" [(ngModel)]="form.label" name="label" placeholder="e.g. Father's Name" required />
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Field Type</label>
            <select class="form-control" [(ngModel)]="form.fieldType" name="fieldType">
              <option value="Text">Text</option>
              <option value="Number">Number</option>
              <option value="Date">Date</option>
              <option value="Email">Email</option>
              <option value="Phone">Phone</option>
              <option value="Select">Dropdown</option>
              <option value="Textarea">Text Area</option>
              <option value="File">File Upload</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">Sort Order</label>
            <input class="form-control" type="number" [(ngModel)]="form.sortOrder" name="sortOrder" min="0" />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label class="form-label d-flex align-items-center gap-2">
              <input type="checkbox" [(ngModel)]="form.isRequired" name="isRequired" />
              Required
            </label>
          </div>
          <div class="form-group">
            <label class="form-label d-flex align-items-center gap-2">
              <input type="checkbox" [(ngModel)]="form.isVisible" name="isVisible" />
              Visible
            </label>
          </div>
        </div>
        @if (form.fieldType === 'Select') {
          <div class="form-group">
            <label class="form-label">Options (JSON array)</label>
            <textarea class="form-control" [(ngModel)]="form.optionsJson" name="optionsJson" rows="3"
              placeholder='["Option 1", "Option 2", "Option 3"]'></textarea>
          </div>
        }
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
export class ProfileFieldConfigComponent implements OnInit {
  fields = signal<ProfileField[]>([]);
  loading = signal(false);
  showModal = signal(false);
  editing = signal(false);
  saving = signal(false);
  editId = 0;

  form: any = this.emptyForm();

  constructor(private api: ApiService, private toast: ToastService) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.api.get<ProfileField[]>('profile-field-config').subscribe({
      next: (result: ProfileField[]) => {
        const items = Array.isArray(result) ? result : [];
        this.fields.set(items.sort((a: ProfileField, b: ProfileField) => a.sortOrder - b.sortOrder));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  openCreate(): void {
    this.form = this.emptyForm();
    this.editing.set(false);
    this.editId = 0;
    this.showModal.set(true);
  }

  openEdit(field: ProfileField): void {
    this.form = { ...field };
    this.editing.set(true);
    this.editId = field.profileFieldConfigId;
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onSave(): void {
    if (!this.form.fieldName || !this.form.label) {
      this.toast.error('Field name and label are required.');
      return;
    }
    this.saving.set(true);
    const payload = { ...this.form };
    const obs = this.editing()
      ? this.api.update('profile-field-config', this.editId, payload)
      : this.api.create('profile-field-config', payload);

    obs.subscribe({
      next: () => {
        this.toast.success(this.editing() ? 'Field updated.' : 'Field created.');
        this.closeModal();
        this.saving.set(false);
        this.loadData();
      },
      error: () => this.saving.set(false),
    });
  }

  onDelete(field: ProfileField): void {
    if (!confirm(`Delete field "${field.label}"?`)) return;
    this.api.delete('profile-field-config', field.profileFieldConfigId).subscribe({
      next: () => {
        this.toast.success('Field deleted.');
        this.loadData();
      },
    });
  }

  private emptyForm(): any {
    return {
      fieldName: '', label: '', fieldType: 'Text', isRequired: false, isVisible: true,
      sortOrder: 0, optionsJson: null,
    };
  }
}
