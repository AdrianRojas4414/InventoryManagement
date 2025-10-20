import { Component, EventEmitter, Input, Output, OnInit, OnChanges, SimpleChanges } from '@angular/core';
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
export class ProductFormComponent implements OnInit, OnChanges {
  @Input() product: Product & { categoryId?: number } = {} as Product;
  @Input() categories: Category[] = [];

  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<CreateProductDto>();
  @Output() openCategory = new EventEmitter<void>();

  editMode = false;
  userId = Number(localStorage.getItem('userId'));
  errorMessage = '';
  successMessage = '';

  showCategoryDropdown = false;
  categorySearchTerm = '';

  constructor(private productService: ProductService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categories'] && !changes['categories'].firstChange) {
      this.loadActiveCategories();
    }
  }
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
        console.log(dto)

        if (error.status === 400 && error.error) {
          this.errorMessage = error.error; 
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

  toggleCategoryDropdown(): void {
    this.showCategoryDropdown = !this.showCategoryDropdown;
    if (this.showCategoryDropdown) {
      this.categorySearchTerm = '';
    }
  }

  getSelectedCategoryName(): string {
    const category = this.categories.find(c => c.id === this.product.categoryId);
    return category ? category.name : 'Seleccione una categoría';
  }

  getFilteredCategories(): Category[] {
    if (!this.categorySearchTerm) {
      return this.categories;
    }
    return this.categories.filter(c => 
      c.name.toLowerCase().includes(this.categorySearchTerm.toLowerCase())
    );
  }

  selectCategory(category: Category): void {
    this.product.categoryId = category.id;
    this.showCategoryDropdown = false;
  }

  openCategoryForm(): void {
    this.openCategory.emit();
  }


  onCategoryCreated(newCategory: Category) {
    this.categories.push(newCategory);
    this.loadActiveCategories(); 
    this.product.categoryId = newCategory.id; 
    this.showCategoryDropdown = false;
  }

  getNameError(name: any): string | null {
    if (!name) return null;
    const value = this.product.name || '';
    if (!(name.dirty || name.touched)) return null;
    if (name.errors?.['required']) {
      return 'El nombre es obligatorio.';
    }
    if (name.errors?.['minlength']) {
      return 'Debe tener al menos 3 caracteres.';
    }
    if (value.length > 100) {
      return 'Debe tener un máximo de 100 caracteres.';
    }
    if (name.errors?.['pattern']) {
      return 'No se permiten caracteres extraños o sólo números.';
    }
    return null;
  }

  getDescriptionError(description: any): string | null {
  if (!description) return null;
    const value = this.product.description || '';
    if (!(description.dirty || description.touched)) return null;
    if (description.errors?.['required']) {
      return 'La descripción es obligatoria.';
    }
    if (description.errors?.['minlength']) {
      return 'Debe tener al menos 5 caracteres.';
    }
    if (description.errors?.['pattern']) {
      return 'No se permiten caracteres extraños.';
    }
    if (value.length > 500) {
      return 'Debe tener un máximo de 500 caracteres.';
    }
    return null;
  }

  getSerialCodeError(serialCode: any): string | null {
    if (!serialCode) return null;
    const value = this.product.serialCode || '';
    if (!(serialCode.dirty || serialCode.touched)) return null;
    if (serialCode.errors?.['required']) {
      return 'El código serial es obligatorio.';
    }
    if (serialCode.errors?.['pattern']) {
      return 'Solo se debe ingresar numeros.';
    }
    if (serialCode.errors?.['minlength']) {
      return 'Debe ingresar 5 digitos.';
    }
    if (value.length > 5) {
      return 'No debe exceder los 5 digitos.';
    }
    if (+value > 32767) {
      return 'El valor del codigo no debe ser mayor a 32767.';
    }

    return null;
  }

  getStockError(stock: any): string | null {
    if (!stock) return null;
    const value = this.product.totalStock || '';
    if (!(stock.dirty || stock.touched)) return null;
    if (stock.errors?.['required']) {
      return 'El stock es obligatorio.';
    }
    if (stock.errors?.['pattern']) {
      return 'Solo se debe ingresar numeros.';
    }
    if (stock.errors?.['min']) {
      return 'El stock no puede ser negativo.';
    }
    if (+value > 32767) {
      return 'La cantidad no debe superar las 32767 unidades.';
    }

    return null;
  }

}
