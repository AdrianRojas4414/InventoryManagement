import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Product, CreateProductDto } from '../../../../../services/product.service';
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
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.editMode = !!this.product.id;
    if (!this.product.categoryId && this.categories.length > 0) {
      this.product.categoryId = this.categories[0].id!;
    }
  }

  save(): void {
    this.errorMessage = '';
    const name = this.product.name?.trim() || '';
    const description = this.product.description?.trim() || '';

    if (name.length < 3) {
      this.errorMessage = 'El nombre debe tener al menos 3 caracteres.';
      return;
    }
    if (description.length < 5) {
      this.errorMessage = 'La descripción debe tener al menos 5 caracteres.';
      return;
    }
    if (!this.product.categoryId) {
      this.errorMessage = 'Selecciona una categoría.';
      return;
    }

    const dto: CreateProductDto = {
      name,
      description,
      categoryId: this.product.categoryId,
      totalStock: this.product.totalStock
    };

    this.saved.emit(dto);
  }

  cancel(): void {
    this.close.emit();
  }

  openCategoryForm(): void {
    this.openCategory.emit();
  }
}
