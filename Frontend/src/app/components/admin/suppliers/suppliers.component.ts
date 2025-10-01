import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../../sidebar/sidebar.component';
import { CommonModule } from '@angular/common';

interface Supplier {
  id: number;
  name: string;
  nit: string;
  address: string;
  phone: string;
  email: string;
  contactName: string;
  showOptions?: boolean;
  disabled?: boolean;
}

@Component({
  selector: 'app-suppliers',
  templateUrl: './suppliers.component.html',
  styleUrls: ['./suppliers.component.css'],
  standalone: true,
  imports: [FormsModule, SidebarComponent, CommonModule]
})
export class SuppliersComponent {
  suppliers: Supplier[] = [
    { id: 1, name: 'Supplier A', nit: '12345678', address: 'Address A', phone: '111', email: 'a@email.com', contactName: 'Contact A' },
    { id: 2, name: 'Supplier B', nit: '456', address: 'Address B', phone: '222', email: 'b@email.com', contactName: 'Contact B' }
  ];

  showForm = false;
  editMode = false;
  disableMode = false;
  currentSupplier: Supplier = {} as Supplier;
  currentEditSupplier: Supplier | null = null;

  openSupplierForm(): void {
    this.showForm = true;
    this.editMode = false;
    this.currentSupplier = {} as Supplier;
  }

  closeForm(): void {
    this.showForm = false;
    this.editMode = false;
    this.currentEditSupplier = null;
  }

  saveSupplier(): void {
    if (this.editMode && this.currentEditSupplier) {
      Object.assign(this.currentEditSupplier, this.currentSupplier);
    } else {
      const newId = this.suppliers.length ? Math.max(...this.suppliers.map(s => s.id)) + 1 : 1;
      this.suppliers.push({ ...this.currentSupplier, id: newId });
    }
    this.closeForm();
  }

  toggleOptions(supplier: Supplier): void {
    supplier.showOptions = !supplier.showOptions;
  }

  editSupplier(supplier: Supplier): void {
    this.editMode = true;
    this.currentSupplier = { ...supplier };
    this.currentEditSupplier = supplier;
    supplier.showOptions = false;
    this.showForm = true;
  }

  disableSupplier(supplier: Supplier): void {
  this.currentSupplier = supplier;
  this.disableMode = true;
  supplier.showOptions = false;
}

  closeDisableConfirm(): void {
    this.disableMode = false;
    this.currentSupplier = {} as Supplier;
  }

  confirmDisableSupplier(): void {
    if (this.currentSupplier) {
      this.currentSupplier.disabled = true;
    }
    this.closeDisableConfirm();
  }
}