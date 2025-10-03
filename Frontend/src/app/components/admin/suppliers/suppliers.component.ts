import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../../sidebar/sidebar.component';
import { CommonModule } from '@angular/common';
import { SupplierService, Supplier } from '../../../services/supplier.service';

@Component({
  selector: 'app-suppliers',
  templateUrl: './suppliers.component.html',
  styleUrls: ['./suppliers.component.css'],
  standalone: true,
  imports: [FormsModule, SidebarComponent, CommonModule]
})
export class SuppliersComponent implements OnInit {
  suppliers: Supplier[] = [];
  showForm = false;
  editMode = false;
  disableMode = false;

  // Inicializamos currentSupplier con un objeto vacío para evitar null
  currentSupplier: Supplier = {
    id: 0,
    name: '',
    nit: '',
    address: '',
    phone: '',
    email: '',
    contactName: '',
    status: 1
  };

  currentEditId: number | null = null;

  constructor(private supplierService: SupplierService) {}

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.supplierService.getSuppliers().subscribe(
      (data: Supplier[]) => { this.suppliers = data; });
  }

  openForm(supplier?: Supplier): void {
    this.showForm = true;
    if (supplier) {
      this.editMode = true;
      this.currentEditId = supplier.id;
      this.currentSupplier = { ...supplier };
    } else {
      this.editMode = false;
      this.currentEditId = null;
      this.currentSupplier = {
        id: 0,
        name: '',
        nit: '',
        address: '',
        phone: '',
        email: '',
        contactName: '',
        status: 1
      };
    }
  }

  closeForm(): void {
    this.showForm = false;
    this.editMode = false;
    this.currentEditId = null;
    this.currentSupplier = {
      id: 0,
      name: '',
      nit: '',
      address: '',
      phone: '',
      email: '',
      contactName: '',
      status: 1
    };
  }

  saveSupplier(): void {
    if (this.editMode && this.currentEditId) {
      this.supplierService.updateSupplier(this.currentEditId, this.currentSupplier)
        .subscribe((updated: Supplier) => {
          const index = this.suppliers.findIndex(s => s.id === updated.id);
          if (index >= 0) this.suppliers[index] = updated;
          this.closeForm();
        });
    } else {
      this.supplierService.createSupplier(this.currentSupplier, 1) // userId = 1 como ejemplo
        .subscribe((created: Supplier) => {
          this.suppliers.push(created);
          this.closeForm();
        });
    }
  }

  editSupplier(supplier: Supplier): void {
    this.openForm(supplier);
  }

  disableSupplier(supplier: Supplier): void {
    this.currentSupplier = supplier;
    this.disableMode = true;
    supplier.showOptions = false;
  }

  enableSupplier(supplier: Supplier): void {
    this.supplierService.activateSupplier(supplier.id, 'Admin')
      .subscribe(() => {
        supplier.status = 1;
        supplier.showOptions = false; // ocultamos toggle
      });
  }

  closeDisableConfirm(): void {
    this.disableMode = false;
    this.currentSupplier = {
      id: 0,
      name: '',
      nit: '',
      address: '',
      phone: '',
      email: '',
      contactName: '',
      status: 1
    };
  }

  confirmDisableSupplier(): void {
    if (this.currentSupplier.id) {
      this.supplierService.deactivateSupplier(this.currentSupplier.id, 'Admin')
        .subscribe(() => {
          const idx = this.suppliers.findIndex(s => s.id === this.currentSupplier.id);
          if (idx >= 0) this.suppliers[idx].status = 0;
          this.closeDisableConfirm();
        });
    }
  }

  toggleOptions(supplier: Supplier): void {
    this.suppliers.forEach(s => {
      if (s !== supplier) s.showOptions = false;
    });
  
    supplier.showOptions = !supplier.showOptions;
  }
}
