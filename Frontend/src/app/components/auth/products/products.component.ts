import { Component, OnInit } from '@angular/core';
import { CategoryService, Category, PaginatedResponse } from '../../../services/category.service';
import { ProductService, Product, CreateProductDto } from '../../../services/product.service';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ConfirmModalComponent } from '../confirm-modal/confirm-modal.component';

import { AuthService } from '../../../services/auth.service';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { CategoryTableComponent } from './components/category-table/category-table.component';
import { CategoryFormComponent } from './components/category-form/category-form.component';
import { ProductTableComponent } from './components/product-table/product-table.component';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    FormsModule,
    SidebarComponent,
    CommonModule,
    ConfirmModalComponent,
    ProductFormComponent,
    CategoryFormComponent,
    ProductTableComponent,
    CategoryTableComponent
  ],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
  categories: Category[] = [];
  categoriesPaged: Category[] = [];
  products: (Product & { categoryName?: string })[] = [];

  categoryPagination = {
    currentPage: 0,
    pageSize: 5,
    totalItems: 0,
    totalPages: 0
  };

  currentProduct: Product & { categoryId?: number } = {} as Product;
  currentCategory: Category = { name: '', description: '' };

  showProductForm = false;
  showCategoryForm = false;
  disableMode = { type: '', id: 0, active: false };

  userId = Number(localStorage.getItem('userId'));
  userRole: string = '';

  constructor(
    private categoryService: CategoryService,
    private productService: ProductService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadAllCategories();
    this.loadProducts();
    if (this.authService.isAdmin()) {
      this.userRole = 'Admin';
    }
  }

  // 🔹 Cargar todas las categorías
  loadAllCategories(): void {
    this.categoryService.getAllCategories().subscribe({
      next: (data) => (this.categories = data),
      error: (err) => console.error('Error cargando categorías:', err)
    });
  }

  // 🔹 Cargar las categorías paginadas
  loadCategories(page: number = 0): void {
    this.categoryService.getCategories(page, this.categoryPagination.pageSize).subscribe({
      next: (response: PaginatedResponse<Category>) => {
        this.categoriesPaged = response.data;
        console.log(this.categoriesPaged);
        this.categoryPagination = {
          currentPage: response.page,
          pageSize: response.pageSize,
          totalItems: response.total,
          totalPages: response.totalPages
        };
      },
      error: (err) => console.error('Error cargando categorías:', err)
    });
  }

  // 🔹 Navegación de páginas
  onPageChange(page: number): void {
    this.loadCategories(page);
  }

  // 🔹 Cargar productos
  loadProducts(): void {
    this.productService.getProducts().subscribe({
      next: (data) => {
        this.products = data.map(p => ({
          ...p,
          categoryName:
            this.categories.find(c => c.id === p.categoryId)?.name || 'Sin categoría'
        }));
      },
      error: (err) => console.error('Error cargando productos:', err)
    });
  }

  // 🔹 Abrir formularios
  openProductForm(product?: Product): void {
    this.showProductForm = true;
    this.currentProduct = product ? { ...product } : {} as Product;
  }

  openCategoryForm(category?: Category): void {
    this.showCategoryForm = true;
    this.currentCategory = category
      ? { ...category }
      : { name: '', description: '' };
  }

  // 🔹 Cerrar solo uno a la vez
  closeForms(formType?: 'product' | 'category'): void {
    if (formType === 'product') this.showProductForm = false;
    else if (formType === 'category') this.showCategoryForm = false;
    else {
      this.showProductForm = false;
      this.showCategoryForm = false;
    }
  }

  saveCategory(category: Category): void {
  this.categoryService.addCategory(category, this.userId).subscribe({
    next: (createdCategory: Category) => {
      // 🔹 Aseguramos que el ID siempre sea un número válido
      const categoryId = Number(createdCategory.id);

      // 1️⃣ Agrega la nueva categoría a la lista local
      this.categories.push(createdCategory);

      // 2️⃣ Si el formulario de producto está abierto, la asigna automáticamente
      if (this.showProductForm && categoryId > 0) {
        this.currentProduct.categoryId = categoryId;
      }

      // 3️⃣ Cierra solo el formulario de categoría
      this.showCategoryForm = false;

      // 4️⃣ Refresca categorías desde backend
      this.loadCategories();
    },
    error: (err) => console.error('Error al guardar categoría:', err)
  });
  }



  // 🔹 Guardar producto
  saveProduct(newProduct: CreateProductDto): void {
  this.productService.createProduct(newProduct, this.userId).subscribe({
    next: () => {
      this.loadProducts();
      this.closeForms('product');
    },
    error: (err) => {
      console.error('Error al guardar producto:', err);
    }
  });
}



  // 🔹 Confirmación
  openConfirm(type: 'product' | 'category', item: Product | Category) {
    this.disableMode = { type, id: item.id!, active: true };
  }

  confirmDisable() {
    if (this.disableMode.type === 'product') {
      this.productService.desactivate(this.disableMode.id, this.userRole).subscribe({
        next: () => {
          this.loadProducts();
          this.disableMode.active = false;
        },
        error: (err) => console.error('Error al desactivar producto:', err)
      });
    }
  }
}
