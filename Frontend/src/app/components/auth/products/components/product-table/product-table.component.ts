import { Component, EventEmitter, Input, NgModule, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Product, ProductService } from '../../../../../services/product.service';
import { AuthService } from '../../../../../services/auth.service';

@Component({
  selector: 'app-product-table',
  imports: [FormsModule, CommonModule],
  templateUrl: './product-table.component.html',
  styleUrls: ['./product-table.component.css'],
})
export class ProductTableComponent {
  Math = Math;
  constructor(private authService: AuthService, private productService: ProductService) {} 
  @Input() products: (Product & { categoryName?: string })[] = [];
  @Output() edit = new EventEmitter<Product>();
  @Output() disable = new EventEmitter<Product>();
  
  isAdmin = false;
  pageSize = 5;    
  currentPage = 0;   
  pagedCategories: Product[] = [];

  ngOnInit() {
    this.isAdmin = this.authService.isAdmin();
    this.updatePagedProducts();
  }

  ngOnChanges() {
    this.currentPage = 0;
    this.updatePagedProducts();
  }

  toggleOptions(product: Product) {
    product['showOptions'] = !product['showOptions'];
  }

  nextPage() {
    if ((this.currentPage + 1) * this.pageSize < this.products.length) {
      this.currentPage++;
      this.updatePagedProducts();
    }
  }

  prevPage() {
    if (this.currentPage > 0) {
      this.currentPage--;
      this.updatePagedProducts();
    }
  }

  updatePagedProducts() {
    const start = this.currentPage * this.pageSize;
    const end = start + this.pageSize;
    this.pagedCategories = this.products.slice(start, end);
  }

  enableProduct(product: Product) {
    console.log('Habilitando producto ID:', product.id, 'Rol:', localStorage.getItem('role'));
    this.productService.activate(product.id, localStorage.getItem('role')!).subscribe({
      next: () => {
        console.log('Producto habilitado correctamente en la BD');
        product.status = 1; 
      },
      error: (err: any) => console.error('Error al habilitar producto:', err)
    });
  }
}
