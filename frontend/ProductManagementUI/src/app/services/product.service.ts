import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Product } from '@models/product';
import { ProductStore } from '@state/product.store';
import { generateProductId } from '@utils/product.utils';

/**
 * ProductService orchestrates product-related operations
 * Combines form creation and business logic
 */
@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(
    private fb: FormBuilder,
    private productStore: ProductStore,
  ) {}

  /**
   * Create product form with validators
   */
  createProductForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      sku: ['', [Validators.required], [this.createSkuUniqueValidator()]],
      price: [0, [Validators.required, Validators.min(0.01)]],
      stock: [0, [Validators.required, Validators.min(0)]],
      category: ['', Validators.required],
    });
  }

  /**
   * Create SKU unique validator that checks store dynamically
   */
  private createSkuUniqueValidator() {
    return (control: any) => {
      return new Promise((resolve) => {
        setTimeout(() => {
          if (!control.value) {
            resolve(null);
            return;
          }

          const existingSKUs = this.productStore.getAllSKUs();
          const skuExists = existingSKUs.some(
            (sku) => sku.toLowerCase() === control.value?.toLowerCase(),
          );

          resolve(skuExists ? { skuTaken: true } : null);
        }, 300);
      });
    };
  }

  /**
   * Create new product from form values
   */
  createProductFromForm(formValue: any): Product {
    return {
      id: generateProductId(),
      ...formValue,
      createdAt: new Date(),
    };
  }

  /**
   * Add product via store
   */
  addProduct(product: Product): void {
    this.productStore.addProduct(product);
  }

  /**
   * Sell product via store and return success status
   */
  sellProduct(productId: number): boolean {
    return this.productStore.sellProduct(productId);
  }

  /**
   * Get product by ID
   */
  getProductById(productId: number): Product | undefined {
    return this.productStore.products().find((p) => p.id === productId);
  }
}
