# Visual Architecture Guide

## Data Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        USER INTERACTION                         │
│                  (Click, Submit, Select...)                     │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│                  COMPONENT LAYER                                 │
│              (product-list.component.ts)                        │
│  Responsibilities:                                              │
│  - Render UI                                                    │
│  - Handle user events                                           │
│  - Call service methods                                         │
│  - Display signals from store                                   │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                ┌──────────┴──────────┐
                │                     │
                ▼                     ▼
    ┌─────────────────────┐  ┌──────────────────┐
    │  ProductService     │  │  ProductStore    │
    │  (Business Logic)   │  │  (State Mgmt)    │
    ├─────────────────────┤  ├──────────────────┤
    │ - createProductForm │  │ - _products      │
    │ - addProduct        │  │ - _selectedCat   │
    │ - sellProduct       │  │ - filteredProd   │
    │ - getProductById    │  │ - totalItems     │
    └──────────┬──────────┘  │ - totalValue     │
               │             └────────┬─────────┘
               │                      │
               └──────────┬───────────┘
                          │
        ┌─────────────────┴─────────────────┐
        │                                   │
        ▼                                   ▼
    ┌──────────────┐             ┌──────────────────┐
    │ utils/       │             │ core/            │
    │ validators   │             │ constants        │
    ├──────────────┤             ├──────────────────┤
    │ - skuUnique  │             │ - CATEGORY_*     │
    │ - minPrice   │             │ - MESSAGES       │
    │ - validate.. │             │ - THRESHOLDS     │
    └──────────────┘             └──────────────────┘
        │                                   │
        │       ┌─────────────────────────┐ │
        │       │                         │ │
        └──────►│  Signal Updates         │◄┘
                │  (Reactive)             │
                │                         │
                └────────────┬────────────┘
                             │
                             ▼
                   ┌──────────────────┐
                   │  DOM Updated     │
                   │  (Automatic)     │
                   └──────────────────┘
```

## Folder Structure

```
📦 ProductManagementUI
├── 📄 ARCHITECTURE.md                    [Architecture details]
├── 📄 DEVELOPMENT_GUIDE.md               [Development guide]
├── 📄 RESTRUCTURING_SUMMARY.md           [What changed]
├── 📄 README_RESTRUCTURING.md            [Quick start]
├── 📄 package.json
├── 📄 tsconfig.json
├── 📄 tsconfig.app.json                  [Path aliases here]
│
├── 📁 src/
│   ├── 📄 main.ts
│   ├── 📄 index.html
│   ├── 📄 styles.css
│   │
│   ├── 📁 app/
│   │   ├── 📄 app.ts                     [Root component]
│   │   ├── 📄 app.routes.ts              [Routing]
│   │   ├── 📄 app.config.ts              [Config]
│   │   │
│   │   ├── 📁 core/                      [⭐ NEW - Global config]
│   │   │   ├── 📄 constants.ts           [All constants]
│   │   │   ├── 📄 index.ts               [Barrel export]
│   │   │   └── 📋 Contains: Categories, Messages, Thresholds
│   │   │
│   │   ├── 📁 services/                  [⭐ NEW - Business logic]
│   │   │   ├── 📄 product.service.ts     [Product operations]
│   │   │   ├── 📄 index.ts               [Barrel export]
│   │   │   └── 📋 Orchestrates: Forms, validation, operations
│   │   │
│   │   ├── 📁 state/                     [⭐ NEW - State mgmt]
│   │   │   ├── 📄 product.store.ts       [Centralized state]
│   │   │   ├── 📄 index.ts               [Barrel export]
│   │   │   └── 📋 Manages: Products, filtering, computations
│   │   │
│   │   ├── 📁 utils/                     [⭐ NEW - Helpers]
│   │   │   ├── 📄 validators.ts          [Form validators]
│   │   │   ├── 📄 product.utils.ts       [Helper functions]
│   │   │   ├── 📄 index.ts               [Barrel export]
│   │   │   └── 📋 Pure: isOutOfStock, formatCurrency, etc]
│   │   │
│   │   ├── 📁 shared/                    [⭐ NEW - Reusable]
│   │   │   └── 📁 components/
│   │   │       └── 📄 action-button.component.ts [Example]
│   │   │
│   │   ├── 📁 models/                    [Type definitions]
│   │   │   ├── 📄 product.ts             [Product interface]
│   │   │   └── 📄 index.ts               [Barrel export]
│   │   │
│   │   └── 📁 components/                [Feature components]
│   │       └── 📁 product-list/
│   │           ├── 📄 product-list.ts    [✅ Refactored - 50 lines]
│   │           └── 📄 product-list.html  [✅ Updated]
│   │
│   └── 📁 environments/                  [⭐ NEW - Config]
│       ├── 📄 environment.ts             [Production]
│       └── 📄 environment.development.ts [Development]
│
└── 📁 .angular/                          [Build cache]
```

## Dependency Graph

```
Components
   │
   ├─► ProductService
   │       │
   │       ├─► ProductStore
   │       │       │
   │       │       └─► utils/product.utils.ts
   │       │
   │       └─► utils/validators.ts
   │
   └─► ProductStore
           │
           └─► models/Product
                   │
                   └─► core/constants.ts
```

## Component Communication Pattern

```
┌──────────────────────────────────────────────┐
│         ProductListComponent                 │
├──────────────────────────────────────────────┤
│                                              │
│  // Read from store                          │
│  products = this.store.products              │
│  filteredProducts = this.store.filteredProd  │
│                                              │
│  // Call service on events                   │
│  addProduct() {                              │
│    this.service.addProduct(product);         │
│  }                                           │
│                                              │
│  sellProduct(id) {                           │
│    this.service.sellProduct(id);             │
│  }                                           │
│                                              │
└──────────────────────────────────────────────┘
         │                           │
         │ Depends on                │ Updates
         │                           │
         ▼                           ▼
    ┌──────────────────────┐   ┌──────────────────┐
    │ ProductService       │   │ ProductStore     │
    │                      │   │                  │
    │ createProductForm()  │   │ addProduct()     │
    │ addProduct()         │   │ sellProduct()    │
    │ sellProduct()        │   │ setCategory()    │
    └──────────────────────┘   └──────────────────┘
             │                         │
             │ Uses                    │ Uses
             │                         │
             └──────────┬──────────────┘
                        │
            ┌───────────┴───────────┐
            │                       │
            ▼                       ▼
        validators             product.utils
        constants
```

## Reactive Data Flow with Signals

```
User Action (onClick)
        │
        ▼
Component Method
        │
        ▼
Service Method
        │
        ▼
Store Update
    │
    ├─► _products.update(...)      [Signal mutated]
    │
    ▼
Computed Signals Update
    │
    ├─► filteredProducts = computed(...)
    ├─► totalItems = computed(...)
    └─► totalValue = computed(...)
    │
    ▼
Component Template Reads
    │
    ├─► @for (product of filteredProducts(); track product.id)
    │
    ▼
DOM Automatically Updated
    │
    ▼
User Sees Changes ✅
```

## State Management Flow

```
┌─────────────────────────────────────┐
│      ProductStore (State)           │
├─────────────────────────────────────┤
│                                     │
│  Private Signals (Mutable):         │
│  • _products                        │
│  • _selectedCategory                │
│                                     │
│  Public Readonly Signals:           │
│  • products (read-only)             │
│  • selectedCategory (read-only)     │
│                                     │
│  Computed Signals (Derived):        │
│  • filteredProducts                 │
│  • totalItems                       │
│  • totalValue                       │
│                                     │
│  Public Methods:                    │
│  • getAllSKUs()                     │
│  • addProduct()                     │
│  • setSelectedCategory()            │
│  • sellProduct()                    │
└─────────────────────────────────────┘
```

## Import Paths - Before vs After

```
BEFORE (Relative Paths - Hard to Refactor)
├── components/product-list/product-list.ts
│   ├── import { Product } from '../../models/product'
│   ├── import { ProductService } from '../../services/product.service'
│   └── import { CATEGORY_OPTIONS } from '../../core/constants'
│
├── services/product.service.ts
│   ├── import { Product } from '../models/product'
│   └── import { ProductStore } from '../state/product.store'
│
└── utils/validators.ts
    └── import { skuUniqueValidatorFactory } from '../utils/validators'

AFTER (Path Aliases - Consistent & Refactor-Proof)
├── components/product-list/product-list.ts
│   ├── import { Product } from '@models/product'
│   ├── import { ProductService } from '@services/product.service'
│   └── import { CATEGORY_OPTIONS } from '@core/constants'
│
├── services/product.service.ts
│   ├── import { Product } from '@models/product'
│   └── import { ProductStore } from '@state/product.store'
│
└── utils/validators.ts
    └── import { skuUniqueValidatorFactory } from '@utils/validators'
```

## Testing Architecture

```
┌─────────────────────────────────────────────┐
│            Unit Tests                       │
├─────────────────────────────────────────────┤
│                                             │
│  Component Tests                            │
│  ├─ Mock ProductService                    │
│  ├─ Mock ProductStore                      │
│  └─ Test: user actions → service calls      │
│                                             │
│  Service Tests                              │
│  ├─ Mock ProductStore                      │
│  └─ Test: business logic                    │
│                                             │
│  Store Tests                                │
│  └─ Test: state mutations                   │
│                                             │
│  Util Tests                                 │
│  └─ Test: pure functions                    │
│                                             │
│  Validator Tests                            │
│  └─ Test: validation logic                  │
│                                             │
└─────────────────────────────────────────────┘
```

## Directory Tree Commands

```bash
# View structure (Unix/Linux/Mac)
tree src/app -L 2

# View structure (Windows PowerShell)
Get-ChildItem -Path src\app -Recurse -Directory

# View with file sizes
ls -la src/app
```

## Quick Reference Card

```
Path Alias      →  Folder
─────────────────────────────────
@app/*          →  app/
@core/*         →  app/core/
@services/*     →  app/services/
@state/*        →  app/state/
@utils/*        →  app/utils/
@models/*       →  app/models/
@shared/*       →  app/shared/
@components/*   →  app/components/
@env/*          →  environments/
```

## Scaling the Architecture

```
Current Structure (PERFECT FOR)
├── Single product management feature
├── Small to medium team
└── Foundational patterns set

To Add More Features
├── Create feature-specific services
├── Extend state as needed
├── Follow same patterns
└── Reuse shared components

Enterprise Scaling
├── Consider feature modules
├── Add lazy loading
├── Implement error boundary
├── Add logging service
├── Add auth interceptor
└── Add request timeout policy
```

---

**Diagram Legend:**

- 📄 = File
- 📁 = Folder
- ⭐ = NEW (created during restructuring)
- ✅ = Updated/Refactored
- 📋 = Description

Use these diagrams as reference while working with the codebase!
