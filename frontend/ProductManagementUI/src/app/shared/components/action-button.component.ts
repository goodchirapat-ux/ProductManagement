import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Reusable button component demonstrating shared component pattern
 * Can be used across multiple features
 */
@Component({
  selector: 'app-action-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button [disabled]="disabled" [class]="variant" (click)="onClick.emit()">
      {{ label }}
    </button>
  `,
  styles: [],
})
export class ActionButtonComponent {
  @Input() label: string = 'Button';
  @Input() variant: 'primary' | 'danger' | 'success' = 'primary';
  @Input() disabled: boolean = false;
  @Output() onClick = new EventEmitter<void>();
}
