import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../../sidebar/sidebar.component';
import { CommonModule } from '@angular/common';
import { CategoryService, Category } from '../../../services/category.service';
import { ProductService, Product, CreateProductDto } from '../../../services/product.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [FormsModule, SidebarComponent, CommonModule],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent {
  categories: Category[] = [];
  products: (Product & { categoryName?: string })[] = [];

  showProductForm = false;
  editProductMode = false;

  showCategoryForm = false;
  editCategoryMode = false;

  currentProduct: Product & { categoryId?: number } = {} as Product;
  currentEditProduct: Product | null = null;

  currentCategory: Category = { name: '', description: '' };
  currentEditCategory: Category | null = null;

  userId = Number(localStorage.getItem('userId'));

  constructor(private categoryService: CategoryService, private productService: ProductService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe(data => {
      this.categories = data;
      this.loadProducts(); // aseguramos que categories esté cargado antes de mapear productos
    });
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe(data => {
      // Agregamos categoryName para usar en el template
      this.products = data.map(p => ({
        ...p,
        categoryName: this.categories.find(c => c.id === p.categoryId)?.name || 'Unknown'
      }));
    });
  }

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

  saveProduct(): void {
    if (this.editProductMode && this.currentEditProduct) {
      // Aquí iría la lógica de edición (PUT) si el backend lo soporta
    } else {
      const newProduct: CreateProductDto = {
        id: this.currentProduct.id,
        name: this.currentProduct.name,
        description: this.currentProduct.description,
        categoryId: this.currentProduct.categoryId!,
        totalStock: this.currentProduct.totalStock
      };

      this.productService.createProduct(newProduct, this.userId)
        .subscribe(() => {
          this.loadProducts();
          this.closeForm();
        });
    }
  }

  saveCategory(): void {
    if (this.editCategoryMode && this.currentEditCategory) {
      // Lógica de edición si la agregas al backend
    } else {
      this.categoryService.addCategory(this.currentCategory, this.userId)
        .subscribe(() => {
          this.loadCategories();
          this.closeFormCategory();
        });
    }
  }

  resetModals(): void {
    this.showProductForm = false;
    this.editProductMode = false;
    this.showCategoryForm = false;
    this.editCategoryMode = false;
  }

  closeForm(): void {
    this.showProductForm = false;
    this.editProductMode = false;
  }

  closeFormCategory(): void {
    this.showCategoryForm = false;
    this.editCategoryMode = false;
  }
}
