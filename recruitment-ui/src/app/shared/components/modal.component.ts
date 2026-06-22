import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  template: `
    @if (open()) {
      <div class="modal-overlay" (click)="close.emit()">
        <div class="modal" [class]="'modal-' + size()" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h3>{{ title() }}</h3>
            <button class="modal-close" (click)="close.emit()">&times;</button>
          </div>
          <div class="modal-body">
            <ng-content />
          </div>
        </div>
      </div>
    }
  `,
})
export class ModalComponent {
  open = input(false);
  title = input('');
  size = input<'sm' | 'md' | 'lg'>('md');
  close = output<void>();
}
