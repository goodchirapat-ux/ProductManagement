# Development Guide

## Getting Started

### Prerequisites

- Node.js 18+
- npm 11.8.0+
- Angular CLI 21+

### Installation

```bash
cd frontend/ProductManagementUI
npm install
```

### Development Server

```bash
npm start
# or
ng serve
```

Navigate to `http://localhost:4200/`. The app will automatically reload if you change any source files.

## Project Structure Overview

See [ARCHITECTURE.md](./ARCHITECTURE.md) for detailed architecture documentation.

Quick reference:

- **UI Components**: `src/app/components/`
- **Business Logic**: `src/app/services/`
- **State Management**: `src/app/state/`
- **Reusable Logic**: `src/app/utils/`
- **Configuration**: `src/app/core/`
- **Type Definitions**: `src/app/models/`
- **Shared Components**: `src/app/shared/`

## Common Tasks

### Adding a New Feature

1. Create component in `src/app/components/feature-name/`
2. Add business logic to `src/app/services/feature.service.ts`
3. Update store in `src/app/state/` if needed
4. Add constants to `src/app/core/constants.ts`
5. Extract helpers to `src/app/utils/feature.utils.ts`

### Creating Validators

Add custom validators to `src/app/utils/validators.ts`:

```typescript
export const customValidator = (): ValidatorFn => {
  return (control: AbstractControl): ValidationErrors | null => {
    // validation logic
    return null; // or { errorKey: true }
  };
};
```

### Using Path Aliases

Imports are simplified with path aliases:

```typescript
import { Product } from '@models/product';
import { ProductService } from '@services/product.service';
import { CATEGORY_OPTIONS } from '@core/constants';
import { formatCurrency } from '@utils/product.utils';
```

### Accessing Store in Components

```typescript
constructor(
  private productStore: ProductStore,
  private productService: ProductService
) {}

// Read signals
products = this.productStore.products;
filteredProducts = this.productStore.filteredProducts;

// Write to store
addProduct() {
  const product = this.productService.createProductFromForm(this.form.value);
  this.productService.addProduct(product);
}
```

## Testing

### Unit Tests

```bash
ng test
```

### Linting

```bash
ng lint
```

## Building

### Development Build

```bash
ng build
```

### Production Build

```bash
ng build --configuration production
```

## Debugging

### Browser DevTools

- F12 or Ctrl+Shift+I to open DevTools
- Use Angular DevTools extension for signal inspection

### VS Code Debugging

Launch configuration is in `.vscode/launch.json`

### Console Logging

```typescript
console.log('Debug:', this.products());
```

## Signals & Reactivity

### Creating Computed Values

```typescript
// In store
readonly totalItems = computed(() => this.filteredProducts().length);
```

### Updating State

```typescript
// In store
addProduct(product: Product): void {
  this._products.update((current) => [...current, product]);
}
```

### Reading in Components

```typescript
// In component
protected readonly products = this.productStore.products;

// In template
@for (product of products(); track product.id) {
  <!-- render product -->
}
```

## Performance Tips

1. **Use OnPush Change Detection** (when adding components)

   ```typescript
   @Component({
     selector: 'app-list',
     changeDetection: ChangeDetectionStrategy.OnPush
   })
   ```

2. **Track in for loops** (already done)

   ```html
   @for (item of items(); track item.id) { }
   ```

3. **Avoid unnecessary subscriptions** (using signals instead)

4. **Lazy load routes** (when adding routing)

## Common Issues

### Import Errors with Path Aliases

- Make sure `tsconfig.app.json` has the correct path mappings
- Restart VS Code if aliases not working

### Store changes not reflecting in template

- Make sure you're using signals as read-only
- Use `.update()` to mutate state, not direct assignment

### Form validation issues

- Check validators are applied in correct order
- Async validators use `asyncValidators` parameter, not array

## Useful Angular Resources

- [Angular Guide](https://angular.dev/guide)
- [Signals Guide](https://angular.dev/guide/signals)
- [Reactive Forms](https://angular.dev/guide/forms/reactive-forms)
- [Angular Testing](https://angular.dev/guide/testing)

## IDE Setup (VS Code)

### Recommended Extensions

- Angular Language Service
- Prettier - Code formatter
- TypeScript Vue Plugin
- ESLint

### Settings (.vscode/settings.json)

```json
{
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "editor.formatOnSave": true,
  "typescript.tsdk": "node_modules/typescript/lib"
}
```

## Contributing

1. Follow the established folder structure
2. Use path aliases for imports
3. Extract reusable logic to utils or services
4. Add type annotations
5. Keep components focused on presentation
6. Write meaningful variable/function names

## Support

For issues or questions:

1. Check existing code examples
2. Review ARCHITECTURE.md
3. Check Angular documentation
4. Add console logs for debugging
