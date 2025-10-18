import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Product, CreateProductDto, ProductService } from '../../../../../services/product.service';
import { Category } from '../../../../../services/category.service';

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
  @Output() saved = new EventEmitter<CreateProductDto>();
  @Output() openCategory = new EventEmitter<void>();

  editMode = false;
  userId = Number(localStorage.getItem('userId'));
  errorMessage = '';
  successMessage = '';

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.editMode = !!this.product.id;
    if (!this.product.categoryId && this.categories.length > 0) {
      this.product.categoryId = this.categories[0].id!;
    }
  }

  save(): void {
    this.product.name?.trim();
    this.product.description;

    const dto: CreateProductDto = {
      name: this.product.name,
      description: this.product.description,
      categoryId: this.product.categoryId,
      totalStock: this.product.totalStock
    };

    const request = this.editMode
      ? this.productService.updateProduct(this.product.id, dto)
      : this.productService.createProduct(dto, this.userId);

    request.subscribe({
      next: () => {
        this.successMessage = this.editMode
          ? 'Producto actualizado correctamente.'
          : 'Producto agregado correctamente.';

        setTimeout(() => {
          this.saved.emit();
          this.close.emit();
        }, 1000);
      },
      error: (error: any) => {
        console.error('Error al guardar producto:', error);

        if (error.status === 400 && error.error) {
          this.errorMessage = error.error; // mensaje específico del backend
        } else if (error.status === 500) {
          this.errorMessage = 'Error interno del servidor.';
        } else {
          this.errorMessage = 'Error al guardar el producto. Verifica los datos.';
        }
      }
    });
  }

  cancel(): void {
    this.close.emit();
  }

  openCategoryForm(): void {
    this.openCategory.emit();
  }
}
