import { Product } from '@models/product';
import { STOCK_THRESHOLDS } from '@core/constants';

/**
 * Calculate total inventory value
 */
export function calculateTotalInventoryValue(products: Product[]): number {
  return products.reduce((sum, product) => sum + product.price * product.stock, 0);
}

/**
 * Check if product is out of stock
 */
export function isOutOfStock(product: Product): boolean {
  return product.stock === STOCK_THRESHOLDS.OUT_OF_STOCK;
}

/**
 * Check if product has low stock
 */
export function isLowStock(product: Product): boolean {
  return product.stock > 0 && product.stock < STOCK_THRESHOLDS.LOW_STOCK;
}

/**
 * Format currency with Thai Baht symbol
 */
export function formatCurrency(amount: number): string {
  return `${amount.toLocaleString('th-TH', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} ฿`;
}

/**
 * Generate unique product ID (in production, use backend ID)
 */
export function generateProductId(): number {
  return Date.now();
}
