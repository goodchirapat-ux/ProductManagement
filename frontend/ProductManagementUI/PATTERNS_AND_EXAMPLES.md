# Common Patterns & Code Examples

## Pattern 1: Adding a New Feature

### Step 1: Define Constants

```typescript
// src/app/core/constants.ts
export const FEATURE_MESSAGES = {
  SUCCESS: 'Feature operation succeeded!',
  ERROR: 'Feature operation failed!',
};
```

### Step 2: Create Store

```typescript
// src/app/state/feature.store.ts
import { Injectable } from '@angular/core';
import { signal, computed } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FeatureStore {
  private readonly _items = signal<Item[]>([]);
  readonly items = this._items.asReadonly();

  readonly itemCount = computed(() => this._items().length);

  addItem(item: Item): void {
    this._items.update((current) => [...current, item]);
  }
}
```

### Step 3: Create Service

```typescript
// src/app/services/feature.service.ts
import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FeatureStore } from '@state/feature.store';

@Injectable({ providedIn: 'root' })
export class FeatureService {
  constructor(
    private fb: FormBuilder,
    private store: FeatureStore,
  ) {}

  createForm(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
    });
  }

  addItem(item: Item): void {
    this.store.addItem(item);
  }
}
```

### Step 4: Create Component

```typescript
// src/app/components/feature/feature.component.ts
import { Component } from '@angular/core';
import { FeatureService } from '@services/feature.service';
import { FeatureStore } from '@state/feature.store';

@Component({
  selector: 'app-feature',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './feature.component.html',
})
export class FeatureComponent {
  form = this.service.createForm();
  items = this.store.items;

  constructor(
    private service: FeatureService,
    private store: FeatureStore,
  ) {}

  onSubmit(): void {
    if (this.form.valid) {
      this.service.addItem(this.form.value);
    }
  }
}
```

## Pattern 2: Custom Validator

```typescript
// src/app/utils/validators.ts

// Sync Validator
export const phoneValidator = (): ValidatorFn => {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;

    const valid = /^\d{10}$/.test(control.value);
    return valid ? null : { invalidPhone: true };
  };
};

// Async Validator
export const emailUniqueValidatorFactory = (existingEmails: string[]): AsyncValidatorFn => {
  return (control: AbstractControl): Promise<ValidationErrors | null> => {
    return new Promise((resolve) => {
      setTimeout(() => {
        if (!control.value) {
          resolve(null);
          return;
        }

        const exists = existingEmails.includes(control.value.toLowerCase());
        resolve(exists ? { emailTaken: true } : null);
      }, 500);
    });
  };
};
```

**Usage in form:**

```typescript
this.form = this.fb.group({
  phone: ['', [Validators.required, phoneValidator()]],
  email: ['', [Validators.required], [emailUniqueValidatorFactory(existingEmails)]],
});
```

## Pattern 3: Pure Utility Functions

```typescript
// src/app/utils/string.utils.ts

// Capitalize first letter
export function capitalize(str: string): string {
  return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
}

// Truncate string
export function truncate(str: string, length: number, suffix: string = '...'): string {
  if (str.length <= length) return str;
  return str.substring(0, length) + suffix;
}

// Format date
export function formatDate(date: Date, format: string = 'dd/mm/yyyy'): string {
  // Implementation
  return formatted;
}

// Debounce function
export function debounce<T extends (...args: any[]) => any>(
  func: T,
  wait: number,
): (...args: Parameters<T>) => void {
  let timeout: NodeJS.Timeout;

  return function executedFunction(...args: Parameters<T>) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };

    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
}
```

## Pattern 4: Service with HTTP

```typescript
// src/app/services/api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private apiUrl = `${environment.apiUrl}/products`;

  constructor(private http: HttpClient) {}

  getProducts() {
    return this.http.get<Product[]>(this.apiUrl);
  }

  getProduct(id: number) {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  createProduct(product: Product) {
    return this.http.post<Product>(this.apiUrl, product);
  }

  updateProduct(id: number, product: Product) {
    return this.http.put<Product>(`${this.apiUrl}/${id}`, product);
  }

  deleteProduct(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
```

## Pattern 5: Store with Loading State

```typescript
// src/app/state/feature.store.ts
import { Injectable } from '@angular/core';
import { signal, computed } from '@angular/core';

interface FeatureState {
  items: Item[];
  loading: boolean;
  error: string | null;
}

@Injectable({ providedIn: 'root' })
export class FeatureStore {
  private readonly _state = signal<FeatureState>({
    items: [],
    loading: false,
    error: null,
  });

  readonly items = computed(() => this._state().items);
  readonly loading = computed(() => this._state().loading);
  readonly error = computed(() => this._state().error);

  setLoading(loading: boolean): void {
    this._state.update((state) => ({ ...state, loading }));
  }

  setError(error: string | null): void {
    this._state.update((state) => ({ ...state, error }));
  }

  setItems(items: Item[]): void {
    this._state.update((state) => ({ ...state, items, error: null }));
  }
}
```

## Pattern 6: Component Using Async Data

```typescript
// src/app/components/feature/feature.component.ts
import { Component, OnInit } from '@angular/core';
import { ApiService } from '@services/api.service';
import { FeatureStore } from '@state/feature.store';

@Component({
  selector: 'app-feature',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (store.loading()) {
      <p>Loading...</p>
    } @else if (store.error()) {
      <p>Error: {{ store.error() }}</p>
    } @else {
      <ul>
        @for (item of store.items(); track item.id) {
          <li>{{ item.name }}</li>
        }
      </ul>
    }
  `,
})
export class FeatureComponent implements OnInit {
  constructor(
    private api: ApiService,
    protected store: FeatureStore,
  ) {}

  ngOnInit(): void {
    this.loadItems();
  }

  private loadItems(): void {
    this.store.setLoading(true);
    this.api.getItems().subscribe({
      next: (items) => {
        this.store.setItems(items);
      },
      error: (error) => {
        this.store.setError(error.message);
      },
    });
  }
}
```

## Pattern 7: Shared Reusable Component

```typescript
// src/app/shared/components/alert.component.ts
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export type AlertType = 'success' | 'error' | 'warning' | 'info';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (visible) {
      <div [class]="'alert alert-' + type">
        <span>{{ message }}</span>
        @if (dismissible) {
          <button (click)="onClose()">×</button>
        }
      </div>
    }
  `,
  styles: [
    `
      .alert {
        padding: 12px 16px;
        border-radius: 4px;
        margin-bottom: 16px;
        display: flex;
        justify-content: space-between;
        align-items: center;
      }

      .alert-success {
        background-color: #d4edda;
        color: #155724;
      }

      .alert-error {
        background-color: #f8d7da;
        color: #721c24;
      }
    `,
  ],
})
export class AlertComponent {
  @Input() message: string = '';
  @Input() type: AlertType = 'info';
  @Input() visible: boolean = true;
  @Input() dismissible: boolean = true;
  @Output() close = new EventEmitter<void>();

  onClose(): void {
    this.visible = false;
    this.close.emit();
  }
}
```

**Usage:**

```typescript
<app-alert
  message="Operation successful!"
  type="success"
  (close)="onAlertClose()">
</app-alert>
```

## Pattern 8: Form with Dynamic Fields

```typescript
// src/app/services/form.service.ts
export class FormService {
  createDynamicForm(fields: FormField[]): FormGroup {
    const group: { [key: string]: any } = {};

    fields.forEach((field) => {
      group[field.name] = [field.value || '', field.validators || []];
    });

    return this.fb.group(group);
  }
}

interface FormField {
  name: string;
  type: 'text' | 'number' | 'email' | 'select';
  label: string;
  value?: any;
  validators?: any[];
  options?: { value: any; label: string }[];
}
```

## Pattern 9: Error Handling Service

```typescript
// src/app/services/error-handler.service.ts
import { Injectable } from '@angular/core';
import { signal } from '@angular/core';

export interface AppError {
  message: string;
  code: string;
  timestamp: Date;
  context?: any;
}

@Injectable({ providedIn: 'root' })
export class ErrorHandlerService {
  private readonly errors = signal<AppError[]>([]);

  getErrors() {
    return this.errors.asReadonly();
  }

  handleError(error: any): void {
    const appError: AppError = {
      message: error?.message || 'Unknown error occurred',
      code: error?.code || 'UNKNOWN',
      timestamp: new Date(),
      context: error,
    };

    this.errors.update((current) => [...current, appError]);
    console.error('Error:', appError);
  }

  clearErrors(): void {
    this.errors.set([]);
  }

  clearError(index: number): void {
    this.errors.update((current) => current.filter((_, i) => i !== index));
  }
}
```

## Pattern 10: LocalStorage Persistence

```typescript
// src/app/utils/storage.utils.ts
export function saveToStorage<T>(key: string, value: T): void {
  try {
    const json = JSON.stringify(value);
    localStorage.setItem(key, json);
  } catch (error) {
    console.error('Failed to save to storage:', error);
  }
}

export function getFromStorage<T>(key: string, defaultValue: T): T {
  try {
    const json = localStorage.getItem(key);
    return json ? JSON.parse(json) : defaultValue;
  } catch (error) {
    console.error('Failed to retrieve from storage:', error);
    return defaultValue;
  }
}

export function removeFromStorage(key: string): void {
  try {
    localStorage.removeItem(key);
  } catch (error) {
    console.error('Failed to remove from storage:', error);
  }
}

// Usage in store
@Injectable({ providedIn: 'root' })
export class UserStore {
  private readonly _user = signal<User | null>(getFromStorage<User>('currentUser', null));

  setUser(user: User): void {
    this._user.set(user);
    saveToStorage('currentUser', user);
  }
}
```

---

These patterns can be combined and extended to build complex features while maintaining the clean, scalable architecture!
