import { Component, OnInit } from '@angular/core';
import { CategoryService, Category } from '../../../services/category.service';
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
  products: (Product & { categoryName?: string })[] = [];

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
    this.loadProducts()
    if (this.authService.isAdmin()) {
      this.userRole = 'Admin';
    }
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (data) => (this.categories = data),
      error: (err) => console.error('Error cargando categorías:', err)
    });
  }

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
      const categoryId = Number(createdCategory.id);
      this.categories.push(createdCategory);
      if (this.showProductForm && categoryId > 0) {
        this.currentProduct.categoryId = categoryId;
      }

      this.showCategoryForm = false;
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
