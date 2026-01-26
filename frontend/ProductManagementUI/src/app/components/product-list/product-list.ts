import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Category, Product } from '../../models/product';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList {
  productForm: FormGroup;
  categories = Object.values(Category);

  selectedCategory = signal<string>('');

  constructor(private fb: FormBuilder) {
    // สร้าง Form พร้อม Validation
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      sku: ['', [Validators.required], [this.skuUniqueValidator.bind(this)]], // Async Validator
      price: [0, [Validators.required, Validators.min(0.01)]],
      stock: [0, [Validators.required, Validators.min(0)]],
      category: ['', Validators.required],
    });
  }

  // Custom Validator ตรวจสอบ SKU ซ้ำ
  async skuUniqueValidator(control: AbstractControl): Promise<ValidationErrors | null> {
    const isExisted = this.products().some(
      (p) => p.sku.toLowerCase() === control.value?.toLowerCase(),
    );
    return isExisted ? { skuTaken: true } : null;
  }

  addProduct() {
    if (this.productForm.valid) {
      const newProduct: Product = {
        id: Date.now(), // ใช้ timestamp เป็น ID ชั่วคราว
        ...this.productForm.value,
        createdAt: new Date(),
      };

      // อัปเดต Signal
      this.products.update((prev) => [...prev, newProduct]);

      // Reset Form
      this.productForm.reset({ price: 0, stock: 0, category: '' });
      alert('เพิ่มสินค้าเรียบร้อยแล้ว!');
    }
  }

  products = signal<Product[]>([
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

  filteredProducts = computed(() => {
    const category = this.selectedCategory();
    const list = this.products();

    if (!category) return list;
    return list.filter((p) => p.category === category);
  });

  totalItems = computed(() => this.filteredProducts().length);

  totalValue = computed(() => {
    return this.filteredProducts().reduce((sum, product) => {
      return sum + product.price * product.stock;
    }, 0);
  });

  sellProduct(productId: number) {
    this.products.update((currentProducts) => {
      return currentProducts.map((product) => {
        if (product.id === productId) {
          if (product.stock > 0) {
            alert(`ขาย ${product.name} สำเร็จ!`);
            return { ...product, stock: product.stock - 1 };
          } else {
            alert(`สินค้า ${product.name} หมดแล้ว ไม่สามารถขายได้`);
          }
        }
        return product;
      });
    });
  }
}
