import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../../sidebar/sidebar.component';
import { CommonModule } from '@angular/common';
import { ConfirmModalComponent } from '../../confirm-modal/confirm-modal.component';
import { CategoryService, Category } from '../../../services/category.service';

interface Product {
  id: number;
  name: string;
  category: string;
  description: string;
  stock: number;
  showOptions?: boolean;
  disabled?: boolean;
}

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [FormsModule, SidebarComponent, CommonModule, ConfirmModalComponent],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})

export class ProductsComponent {
  categories: Category[] = [];

  products: Product[] = [
    { id: 1, name: 'Office Chair Executive', category: 'Furniture', description: 'Bla bla bla', stock: 45 },
    { id: 2, name: 'Laptop Stand Adjustable', category: 'Electronics', description: 'Bla bla bla', stock: 5, disabled: true },
    { id: 3, name: 'Wireless Mouse Ergonomic', category: 'Electronics', description: 'Bla bla bla', stock: 120 },
    { id: 4, name: 'Desk Lamp LED', category: 'Electronics', description: 'Bla bla bla', stock: 0 },
    { id: 5, name: 'Office Chair Executive', category: 'Furniture', description: 'Bla bla bla', stock: 45 },
    { id: 6, name: 'Laptop Stand Adjustable', category: 'Electronics', description: 'Bla bla bla', stock: 5, disabled: true }
  ];

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe(data => this.categories = data);
  }

  showProductForm = false;
  editProductMode = false;
  disableProductMode = false;

  showCategoryForm = false;
  editCategoryMode = false;
  disableCategoryMode = false;

  currentProduct: Product = {} as Product;
  currentEditProduct: Product | null = null;

  currentCategory: Category = { name: '', description: '' };
  currentEditCategory: Category | null = null;

  userId = Number(localStorage.getItem('userId'));

  openProductForm(): void {
    this.resetModals();
    this.showProductForm = true;
    this.editProductMode = false;
    this.currentProduct = {} as Product;
  }

  openCategoryForm(): void {
    this.resetModals();
    this.showCategoryForm = true;
    this.editCategoryMode = false;
    this.currentCategory = { name: '', description: '' };
  }

  editProduct(product: Product): void {
    this.resetModals();
    this.showProductForm = true;
    this.editProductMode = true;
    this.currentProduct = { ...product };
    this.currentEditProduct = product;
  }

  editCategory(category: Category): void {
    this.resetModals();
    this.showCategoryForm = true;
    this.editCategoryMode = true;
    this.currentCategory = { ...category };
    this.currentEditCategory = category;
  }

  saveProduct(): void {
    if (this.editProductMode && this.currentEditProduct) {
      Object.assign(this.currentEditProduct, this.currentProduct);
    } else {
      this.currentProduct.id = this.products.length + 1;
      this.products.push({ ...this.currentProduct });
    }
    this.closeForm();
  }

  saveCategory(): void {
    if (this.editCategoryMode && this.currentEditCategory) {
      // Lógica de edición si la agregas al backend
    } else {
      this.categoryService.addCategory(this.currentCategory, this.userId)
        .subscribe(() => {
          this.loadCategories(); // recargar tabla
          this.closeFormCategory();
        });
    }
  }

  disableProduct(product: Product): void {
    this.resetModals();
    this.disableProductMode = true;
    this.currentProduct = product;
  }

  confirmDisableProduct(): void {
    if (this.currentProduct) {
      this.currentProduct.disabled = true;
    }
    this.closeDisableConfirm();
  }

  resetModals(): void {
    this.showProductForm = false;
    this.editProductMode = false;
    this.disableProductMode = false;

    this.showCategoryForm = false;
    this.editCategoryMode = false;
    this.disableCategoryMode = false;
  }

  closeForm(): void {
    this.showProductForm = false;
    this.editProductMode = false;
  }

  closeFormCategory(): void {
    this.showCategoryForm = false;
    this.editCategoryMode = false;
  }

  closeDisableConfirm(): void {
    this.disableProductMode = false;
  }

  closeDisableConfirmCategory(): void {
    this.disableCategoryMode = false;
  }

  toggleOptions(product: Product): void {
    product.showOptions = !product.showOptions;
  }
}
