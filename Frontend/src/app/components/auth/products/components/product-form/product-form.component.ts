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
    this.loadActiveCategories();
    
  }

  loadActiveCategories(): void{
    const activeCategories = this.categories.filter(cat => cat.status === 1);

    if (!this.product.categoryId && activeCategories.length > 0) {
      this.product.categoryId = activeCategories[0].id!;
    }

    this.categories = activeCategories;
  }


  save(): void {
    this.product.name = this.product.name.trim();
    this.product.description = this.product.description.trim();
    

    if (this.product.name.length < 3) {
      this.errorMessage = 'El nombre debe tener al menos 3 caracteres válidos.';
      return;
    }
    if (this.product.description.length < 5) {
      this.errorMessage = 'La descripción debe tener al menos 5 caracteres válidos.';
      return;
    }

    const dto: CreateProductDto = {
      name: this.product.name,
      description: this.product.description,
      categoryId: this.product.categoryId,
      totalStock: this.product.totalStock,
      serialCode: Number(this.product.serialCode),
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

  // Llamado desde el modal de categoría
  onCategoryCreated(newCategory: Category) {
    this.categories.push(newCategory);
    this.loadActiveCategories(); // recarga la lista activa y asigna al select
    this.product.categoryId = newCategory.id; // opcional: seleccionar la nueva
  }

}
