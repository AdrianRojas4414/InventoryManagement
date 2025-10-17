import { Component, OnInit } from '@angular/core';
import { CategoryService, Category } from '../../../services/category.service';
import { ProductService, Product, CreateProductDto } from '../../../services/product.service';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ConfirmModalComponent } from '../confirm-modal/confirm-modal.component';

import { AuthService } from '../../../services/auth.service';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { CategoryFormComponent } from './components/category-form/category-form.component';
import { ProductTableComponent } from './components/product-table/product-table.component';
import { CategoryTableComponent } from './components/category-table/category-table.component';

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

  // Estados de los formularios y confirmaciones
  showProductForm = false;
  showCategoryForm = false;
  disableMode = { type: '', id: 0, active: false };

  userId = Number(localStorage.getItem('userId'));
  userRole : string = '';

  constructor(private categoryService: CategoryService, private productService: ProductService, private authService: AuthService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe(data => {
      this.categories = data;
      this.loadProducts(); // Mapear categorías en productos
    });
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe(data => {
      this.products = data.map(p => ({
        ...p,
        categoryName: this.categories.find(c => c.id === p.categoryId)?.name || 'Unknown'
      }));
    });
  }

  // Abrir formularios
  openProductForm(product?: Product): void {
    this.showProductForm = true;
    this.currentProduct = product ? { ...product } : {} as Product;
  }

  openCategoryForm(category?: Category): void {
    this.showCategoryForm = true;
    this.currentCategory = category ? { ...category } : { name: '', description: '' };
  }

  closeForms(): void {
    this.showProductForm = false;
    this.showCategoryForm = false;
  }

  saveProduct(product: Product & { categoryId?: number }) {
    const newProduct: CreateProductDto = {
      name: product.name,
      description: product.description,
      categoryId: product.categoryId,
      totalStock: product.totalStock
    };

    this.productService.createProduct(newProduct, this.userId).subscribe(() => {
      this.loadProducts();
      this.closeForms();
    });
  }

  saveCategory(category: Category) {
    this.categoryService.addCategory(category, this.userId).subscribe(() => {
      this.loadCategories();
      this.closeForms();
    });
  }

  openConfirm(type: 'product' | 'category', item: Product | Category) {
    this.disableMode = {
      type,
      id: item.id!,
      active: true
    };
  }

  confirmDisable() {
    if (this.disableMode.type === 'product') {
      this.productService.desactivate(this.disableMode.id, this.userRole).subscribe({
        next: () => {
          console.log('Producto desactivado');
          this.loadProducts();
          this.disableMode.active = false;
        },
        error: (err) => console.error('Error al desactivar producto:', err)
      });
    }
  }
}
