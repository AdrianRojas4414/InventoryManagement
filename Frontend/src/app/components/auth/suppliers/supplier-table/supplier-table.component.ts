import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Supplier, SupplierService } from '../../../../services/supplier.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-supplier-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './supplier-table.component.html',
  styleUrls: ['./supplier-table.component.css']
})
export class SupplierTableComponent {
  Math = Math;
  constructor(private authService: AuthService, private supplierService: SupplierService) {}
  @Input() suppliers: Supplier[] = [];
  @Input() isAdmin = true;

  @Output() edit = new EventEmitter<Supplier>();
  @Output() disable = new EventEmitter<Supplier>();
  @Output() enable = new EventEmitter<Supplier>();

  pageSize = 5;    
  currentPage = 0;   
  pagedSuppliers: Supplier[] = [];

  ngOnInit() {
    this.updatePagedSuppliers();
    this.isAdmin = this.authService.isAdmin();
  }

  ngOnChanges() {
    this.currentPage = 0;
    this.updatePagedSuppliers();
  }

  toggleOptions(supplier: Supplier) {
    supplier['showOptions'] = !supplier['showOptions'];
  }

  nextPage() {
    if ((this.currentPage + 1) * this.pageSize < this.suppliers.length) {
      this.currentPage++;
      this.updatePagedSuppliers();
    }
  }

  prevPage() {
    if (this.currentPage > 0) {
      this.currentPage--;
      this.updatePagedSuppliers();
    }
  }

  updatePagedSuppliers() {
    const start = this.currentPage * this.pageSize;
    const end = start + this.pageSize;
    this.pagedSuppliers = this.suppliers.slice(start, end);
  }

  enableSupplier(supplier: Supplier) {
      console.log('Habilitando proveedor ID:', supplier.id, 'Rol:', localStorage.getItem('role'));
      this.supplierService.activateSupplier(supplier.id, localStorage.getItem('role')!).subscribe({
        next: () => {
          console.log('Producto habilitado correctamente en la BD');
          supplier.status = 1; 
        },
        error: (err: any) => console.error('Error al habilitar producto:', err)
      });
    }
}
