export enum Category {
  FOOD = 'อาหาร',
  DRINK = 'เครื่องดื่ม',
  HOUSEHOLD = 'ของใช้',
  CLOTHING = 'เสื้อผ้า',
}

export const CATEGORY_OPTIONS = Object.values(Category);

export const PRODUCT_FORM_ERRORS = {
  NAME_REQUIRED: 'ชื่อสินค้าต้องไม่ว่าง',
  NAME_MIN_LENGTH: 'ชื่อสินค้าต้องมีอย่างน้อย 3 ตัวอักษร',
  SKU_REQUIRED: 'กรุณาระบุ SKU',
  SKU_TAKEN: 'SKU นี้มีในระบบแล้ว',
  PRICE_REQUIRED: 'กรุณาระบุราคา',
  PRICE_MIN: 'ราคาต้องมากกว่า 0',
  STOCK_REQUIRED: 'กรุณาระบุจำนวนสต็อก',
  STOCK_MIN: 'สต็อกต้องไม่ติดลบ',
  CATEGORY_REQUIRED: 'กรุณาเลือกหมวดหมู่',
};

export const SUCCESS_MESSAGES = {
  PRODUCT_ADDED: 'เพิ่มสินค้าเรียบร้อยแล้ว!',
  PRODUCT_SOLD: 'ขาย {productName} สำเร็จ!',
  PRODUCT_OUT_OF_STOCK: 'สินค้า {productName} หมดแล้ว ไม่สามารถขายได้',
};

export const STOCK_THRESHOLDS = {
  LOW_STOCK: 10,
  OUT_OF_STOCK: 0,
};
