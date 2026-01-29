import { AbstractControl, ValidationErrors, ValidatorFn, AsyncValidatorFn } from '@angular/forms';

/**
 * Validates that the input has a minimum length
 */
export const minLengthValidator = (minLength: number): ValidatorFn => {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }
    return control.value.length >= minLength ? null : { minLength: { minLength } };
  };
};

/**
 * Validates that the price is greater than a minimum value
 */
export const minPriceValidator = (minPrice: number): ValidatorFn => {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }
    return parseFloat(control.value) >= minPrice ? null : { minPrice: { minPrice } };
  };
};

/**
 * Validates that stock is non-negative
 */
export const nonNegativeValidator = (): ValidatorFn => {
  return (control: AbstractControl): ValidationErrors | null => {
    if (control.value === null || control.value === undefined) {
      return null;
    }
    return parseInt(control.value, 10) >= 0 ? null : { negative: true };
  };
};

/**
 * Async validator to check if SKU already exists
 */
export const skuUniqueValidatorFactory = (existingSKUs: string[]): AsyncValidatorFn => {
  return (control: AbstractControl): Promise<ValidationErrors | null> => {
    return new Promise((resolve) => {
      setTimeout(() => {
        if (!control.value) {
          resolve(null);
          return;
        }

        const skuExists = existingSKUs.some(
          (sku) => sku.toLowerCase() === control.value?.toLowerCase(),
        );

        resolve(skuExists ? { skuTaken: true } : null);
      }, 300); // Simulate network delay
    });
  };
};
