import { Category } from '@core/constants';

export interface Product {
  id: number;
  name: string;
  sku: string;
  price: number;
  stock: number;
  category: Category;
  createdAt: Date;
}
