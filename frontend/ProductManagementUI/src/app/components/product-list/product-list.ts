import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Product } from '@models/product';
import { ProductService } from '@services/product.service';
import { ProductStore } from '@state/product.store';
import { CATEGORY_OPTIONS, SUCCESS_MESSAGES } from '@core/constants';
import { isOutOfStock, isLowStock, formatCurrency } from '@utils/product.utils';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList implements OnInit {
  productForm!: FormGroup;
  categories = CATEGORY_OPTIONS;

  // Expose store signals to template via getters
  get products() {
    return this.productStore.products;
  }

  get selectedCategory() {
    return this.productStore.selectedCategory;
  }

  get filteredProducts() {
    return this.productStore.filteredProducts;
  }

  get totalItems() {
    return this.productStore.totalItems;
  }

  get totalValue() {
    return this.productStore.totalValue;
  }

  // Expose utility functions to template
  protected readonly isOutOfStock = isOutOfStock;
  protected readonly isLowStock = isLowStock;
  protected readonly formatCurrency = formatCurrency;

  constructor(
    private productService: ProductService,
    private productStore: ProductStore,
  ) {}

  ngOnInit(): void {
    this.productForm = this.productService.createProductForm();
  }

  addProduct(): void {
    if (this.productForm.valid) {
      const newProduct = this.productService.createProductFromForm(this.productForm.value);
      this.productService.addProduct(newProduct);

      this.productForm.reset({ price: 0, stock: 0, category: '' });
      alert(SUCCESS_MESSAGES.PRODUCT_ADDED);
    }
  }

  sellProduct(productId: number): void {
    const product = this.productStore.products().find((p) => p.id === productId);

    if (!product) return;

    const sold = this.productService.sellProduct(productId);

    if (sold) {
      alert(SUCCESS_MESSAGES.PRODUCT_SOLD.replace('{productName}', product.name));
    } else {
      alert(SUCCESS_MESSAGES.PRODUCT_OUT_OF_STOCK.replace('{productName}', product.name));
    }
  }

  onCategoryChange(category: string): void {
    this.productStore.setSelectedCategory(category);
  }
}
