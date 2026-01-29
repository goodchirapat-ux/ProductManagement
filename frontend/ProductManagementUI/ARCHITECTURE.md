# ProductManagementUI - Architecture Guide

This document outlines the restructured Angular application following industry best practices and SOLID principles.

## 📁 Project Structure

```
src/app/
├── core/                    # Core/global application logic
│   └── constants.ts        # App-wide constants, enums, config values
├── services/               # Business logic layer
│   └── product.service.ts  # Product domain service
├── state/                  # State management (signals-based)
│   └── product.store.ts    # Centralized product state
├── shared/                 # Reusable components & utilities
│   └── (future: shared components)
├── utils/                  # Pure utility functions
│   ├── validators.ts       # Form validators (pure functions)
│   └── product.utils.ts    # Product helper functions
├── models/                 # TypeScript interfaces & types
│   └── product.ts          # Product domain model
├── components/             # Feature components
│   └── product-list/       # Product list component
├── app.ts                  # Root component
├── app.routes.ts           # Route configuration
└── app.config.ts           # App configuration & providers

src/
└── environments/           # Environment-specific configs
    ├── environment.ts      # Production environment (to be created)
    └── environment.dev.ts  # Development environment (to be created)
```

## 🏗️ Architecture Principles

### 1. **Separation of Concerns**

- **Components**: Handle UI rendering and user interaction
- **Services**: Manage business logic
- **State**: Centralize application state (signals)
- **Utils**: Pure, reusable utility functions
- **Constants**: Centralize magic numbers, strings, configs

### 2. **Dependency Injection**

All services are provided at the root level using `providedIn: 'root'` for singleton instances:

```typescript
@Injectable({ providedIn: 'root' })
export class ProductService {}
```

### 3. **Signal-Based State Management**

Using Angular 18+ signals for fine-grained reactivity:

- Private signals for internal state
- Public read-only computed signals for templates
- No manual subscription management needed

### 4. **Validators as Pure Functions**

Form validators are extracted to utility functions:

```typescript
// utils/validators.ts
export const skuUniqueValidatorFactory = (existingSKUs: string[]): AsyncValidatorFn => {
  return (control: AbstractControl): Promise<ValidationErrors | null> => {
    // Validator logic
  };
};
```

### 5. **Type Safety**

- Strict TypeScript configuration
- Clear interfaces for domain models
- Path aliases for cleaner imports

## 🔍 Key Files & Responsibilities

### `core/constants.ts`

Centralized constants for:

- Enums (Category)
- Error messages
- Success messages
- Configuration thresholds

**Why**: Single source of truth for text, making UI updates easier

### `state/product.store.ts`

Manages product data using signals:

- `_products`: Private signal with product data
- `filteredProducts`: Computed signal for filtered results
- Public methods: `addProduct()`, `sellProduct()`, `setSelectedCategory()`

**Why**: Separates state logic from component logic

### `services/product.service.ts`

Orchestrates product operations:

- Form creation with validators
- Product creation from form data
- Delegates to store for state mutations

**Why**: Single responsibility - coordinates service-to-store communication

### `utils/validators.ts`

Pure validator functions:

- `skuUniqueValidatorFactory()`: Async validation
- `minPriceValidator()`: Sync validation
- `nonNegativeValidator()`: Stock validation

**Why**: Reusable, testable, framework-agnostic

### `utils/product.utils.ts`

Pure helper functions:

- `calculateTotalInventoryValue()`
- `isOutOfStock()`
- `isLowStock()`
- `formatCurrency()`

**Why**: Decoupled business logic from template logic

### `components/product-list/`

UI component responsibilities:

- Display product list
- Show form
- Call service methods on user action
- Read state from signals

**Why**: Component focuses only on presentation

## 📐 Data Flow

```
User Interaction
       ↓
Component Method (e.g., addProduct())
       ↓
ProductService (orchestration)
       ↓
ProductStore (state mutation)
       ↓
Signals updated → Templates automatically re-render
```

## 🔗 Path Aliases

Import paths are simplified using aliases defined in `tsconfig.app.json`:

```typescript
// Before
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';

// After
import { Product } from '@models/product';
import { ProductService } from '@services/product.service';
```

**Available Aliases:**

- `@app/*` → `app/`
- `@core/*` → `app/core/`
- `@services/*` → `app/services/`
- `@state/*` → `app/state/`
- `@shared/*` → `app/shared/`
- `@utils/*` → `app/utils/`
- `@models/*` → `app/models/`
- `@components/*` → `app/components/`
- `@env/*` → `environments/`

## 🧪 Testing Strategy

### Unit Testing

- **Utils**: Pure functions are easy to test
- **Validators**: Test with different form states
- **Services**: Mock store dependencies

### Component Testing

- Mock ProductStore and ProductService
- Test user interactions and event handlers

## 🚀 Future Improvements

### Already Structured For:

1. **Http Interceptors** - Add to `services/` folder
2. **Guards** - Add to `core/` folder
3. **Pipes** - Add to `shared/` folder
4. **Directives** - Add to `shared/` folder
5. **Environment Config** - Use `environments/` folder
6. **Feature Modules** - Can be added at root level with same structure

### Recommended Next Steps:

1. Create HTTP service layer for backend API calls
2. Add error handling with custom error interceptor
3. Implement logging service
4. Add loading/error states to store
5. Create shared UI components (buttons, forms, tables)
6. Add end-to-end tests with Cypress

## 📋 Best Practices Applied

✅ Single Responsibility Principle - Each file/class has one reason to change
✅ DRY (Don't Repeat Yourself) - Shared logic extracted to utils/services
✅ Dependency Injection - Services injected via constructor
✅ Type Safety - Full TypeScript strict mode
✅ Reactive Programming - Signals for state management
✅ Clean Code - Meaningful names, documented functions
✅ Scalable Structure - Easy to add new features
✅ Testability - Pure functions and dependency injection

## 🔄 Migration Notes

If updating from the old structure:

1. Components now depend on ProductService instead of managing state directly
2. Form validators moved to `utils/validators.ts`
3. Constants moved to `core/constants.ts`
4. Store handles all state mutations
5. Components are now "dumb" (presentational) with service dependencies

## 📚 Related Files

- [Angular Best Practices](https://angular.dev/guide/styleguide)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Signals API](https://angular.dev/guide/signals)
