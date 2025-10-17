import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CreateProductDto, Product, ProductService } from '../../../../../services/product.service';
import { Category, CategoryService } from '../../../../../services/category.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css'],
  imports: [FormsModule, CommonModule],
  standalone: true
})
export class ProductFormComponent implements OnInit {
  @Input() product: Product & { categoryId?: number } = {} as Product;
  @Input() categories: Category[] = [];

  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  editMode = false;
  userId = Number(localStorage.getItem('userId'));

  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private productService: ProductService, 
    private categoryService: CategoryService
  ) {} 

  ngOnInit(): void {
    this.editMode = !!this.product.id;

    if (!this.product.categoryId && this.categories.length > 0) {
      this.product.categoryId = this.categories[0].id!;
    }
  }

  save(): void {
    this.product.name = this.product.name.trim();
    this.product.description = this.product.description.trim();

    if (!this.product.name || this.product.name.length < 3) {
      this.errorMessage =' El nombre debe tener al menos 3 caracteres válidos';
      return;
    }

    if (!this.product.description || this.product.description.length < 5) {
      this.errorMessage = 'La descripción debe tener al menos 5 caracteres válidos';
      return;
    }

    const productData: CreateProductDto = {
      name: this.product.name,
      description: this.product.description,
      categoryId: this.product.categoryId,
      totalStock: this.product.totalStock
    };

    const request = this.editMode
      ? this.productService.updateProduct(this.product.id, productData)
      : this.productService.createProduct(productData, this.userId);

    request.subscribe({
      next: () => {
        this.successMessage = this.editMode
          ? 'Producto actualizado exitosamente.'
          : 'Producto creado exitosamente.';

        setTimeout(() => {
          this.saved.emit();
          this.close.emit();
        }, 1000);
      },
      error: (error: any) => {
        console.error('Error al guardar producto:', error);
        this.errorMessage = 'Error al guardar el producto';
      }
    });
  }

  cancel(): void {
    this.close.emit();
  }
}
