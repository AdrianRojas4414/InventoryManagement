import { Component, OnInit } from '@angular/core';
import { SupplierService, Supplier, CreateSupplierDto } from '../../../services/supplier.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { SupplierFormComponent } from './supplier-form/supplier-form.component';
import { SupplierTableComponent } from './supplier-table/supplier-table.component';
import { ConfirmModalComponent } from '../confirm-modal/confirm-modal.component';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-suppliers',
  standalone: true,
  imports: [FormsModule, CommonModule, SidebarComponent, SupplierFormComponent, SupplierTableComponent, ConfirmModalComponent],
  templateUrl: './suppliers.component.html',
  styleUrls: ['./suppliers.component.css']
})
export class SuppliersComponent implements OnInit {
  suppliers: Supplier[] = [];
  currentSupplier: Supplier = {} as Supplier;

  showForm = false;
  editMode = false;

  disableMode = { id: 0, name: '', active: false };

  userId = Number(localStorage.getItem('userId'));
  userRole : string = '';

  constructor(private supplierService: SupplierService, private authService: AuthService) {}

  ngOnInit(): void {
    this.loadSuppliers();
    if(this.authService.isAdmin()){
      this.userRole = "Admin"
    }
  }

  loadSuppliers(): void {
    this.supplierService.getSuppliers().subscribe({
      next: data => this.suppliers = data,
      error: err => console.error('Error al cargar proveedores:', err)
    });
  }

  openForm(supplier?: Supplier): void {
    this.showForm = true;
    this.editMode = !!supplier;
    this.currentSupplier = supplier ? { ...supplier } : {} as Supplier;
  }

  closeForm(): void {
    this.showForm = false;
  }

  saveSupplier(supplier: Supplier): void {
    if (this.editMode) {
      this.supplierService.updateSupplier(supplier.id, supplier).subscribe({
        next: () => {
          console.log('Proveedor actualizado');
          this.loadSuppliers();
          this.closeForm();
        },
        error: err => console.error('Error al actualizar proveedor:', err)
      });
    } else {
      this.supplierService.createSupplier(supplier, this.userId).subscribe({
        next: () => {
          console.log('Proveedor creado');
          this.loadSuppliers();
          this.closeForm();
        },
        error: err => console.error('Error al crear proveedor:', err)
      });
    }
  }

  openConfirm(supplier: Supplier): void {
    this.currentSupplier = supplier;
    this.disableMode = {
      id: supplier.id,
      name: supplier.name,
      active: true
    };
  }

  confirmDisable() {
    this.supplierService.deactivateSupplier(this.disableMode.id, this.userRole).subscribe({
      next: () => {
        console.log('Proveedor desactivado');
        this.loadSuppliers();
        this.disableMode.active = false;
      },
      error: err => console.error('Error al desactivar proveedor:', err)
    });
  }
}