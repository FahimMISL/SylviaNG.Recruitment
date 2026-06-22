import { Component } from '@angular/core';
import { ToastService } from '../services/toast.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  template: `
    <div class="toast-container">
      @for (toast of toastService.toasts(); track toast.id) {
        <div class="toast" (click)="toastService.dismiss(toast.id)">
          <span class="badge"
            [class.badge-success]="toast.type === 'success'"
            [class.badge-danger]="toast.type === 'error'"
            [class.badge-warning]="toast.type === 'warning'"
            [class.badge-info]="toast.type === 'info'">
            {{ toast.type }}
          </span>
          <span>{{ toast.message }}</span>
        </div>
      }
    </div>
  `,
})
export class ToastContainerComponent {
  constructor(public toastService: ToastService) {}
}
