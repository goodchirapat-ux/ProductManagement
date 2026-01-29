import { Injectable } from '@angular/core';
import { signal, computed } from '@angular/core';
import { Product } from '@models/product';
import { Category } from '@core/constants';

/**
 * ProductStore provides centralized state management for products
 * Using Angular signals for reactive, fine-grained reactivity
 */
@Injectable({
  providedIn: 'root',
})
export class ProductStore {
  // Private signals - internal state
  private readonly _products = signal<Product[]>([
    {
      id: 1,
      name: 'ข้าวหอมมะลิ 5กก.',
      sku: 'FOOD-001',
      price: 185,
      stock: 40,
      category: Category.FOOD,
      createdAt: new Date(),
    },
    {
      id: 2,
      name: 'น้ำดื่มบริสุทธิ์ 600มล.',
      sku: 'DRK-002',
      price: 7,
      stock: 120,
      category: Category.DRINK,
      createdAt: new Date(),
    },
    {
      id: 3,
      name: 'ผงซักฟอก สูตรเข้มข้น',
      sku: 'HSE-003',
      price: 129,
      stock: 25,
      category: Category.HOUSEHOLD,
      createdAt: new Date(),
    },
    {
      id: 4,
      name: 'กางเกงยีนส์ขายาว',
      sku: 'CLS-004',
      price: 590,
      stock: 5,
      category: Category.CLOTHING,
      createdAt: new Date(),
    },
    {
      id: 5,
      name: 'บะหมี่กึ่งสำเร็จรูป แพ็ค 10',
      sku: 'FOOD-005',
      price: 60,
      stock: 100,
      category: Category.FOOD,
      createdAt: new Date(),
    },
    {
      id: 6,
      name: 'เสื้อเชิ้ตลายสก๊อต',
      sku: 'CLS-006',
      price: 350,
      stock: 0,
      category: Category.CLOTHING,
      createdAt: new Date(),
    },
  ]);

  private readonly _selectedCategory = signal<string>('');

  // Public computed signals - derived state
  readonly products = this._products.asReadonly();
  readonly selectedCategory = this._selectedCategory.asReadonly();

  readonly filteredProducts = computed(() => {
    const category = this._selectedCategory();
    const products = this._products();

    if (!category) return products;
    return products.filter((p) => p.category === category);
  });

  readonly totalItems = computed(() => this.filteredProducts().length);

  readonly totalValue = computed(() => {
    return this.filteredProducts().reduce((sum, product) => {
      return sum + product.price * product.stock;
    }, 0);
  });

  /**
   * Get all existing SKUs (for validation)
   */
  getAllSKUs(): string[] {
    return this._products().map((p) => p.sku);
  }

  /**
   * Add a new product to the store
   */
  addProduct(product: Product): void {
    this._products.update((current) => [...current, product]);
  }

  /**
   * Update selected category for filtering
   */
  setSelectedCategory(category: string): void {
    this._selectedCategory.set(category);
  }

  /**
   * Sell a product (decrease stock)
   */
  sellProduct(productId: number): boolean {
    let sold = false;

    this._products.update((current) =>
      current.map((product) => {
        if (product.id === productId) {
          if (product.stock > 0) {
            sold = true;
            return { ...product, stock: product.stock - 1 };
          }
        }
        return product;
      }),
    );

    return sold;
  }
}
