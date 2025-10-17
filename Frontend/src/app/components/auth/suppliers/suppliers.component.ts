import { Component, OnInit } from '@angular/core';
import { SupplierService, Supplier, CreateSupplierDto } from '../../../services/supplier.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { SupplierFormComponent } from './supplier-form/supplier-form.component';
import { SupplierTableComponent } from './supplier-table/supplier-table.component';

@Component({
  selector: 'app-suppliers',
  standalone: true,
  imports: [FormsModule, CommonModule, SidebarComponent, SupplierFormComponent, SupplierTableComponent],
  templateUrl: './suppliers.component.html',
  styleUrls: ['./suppliers.component.css']
})
export class SuppliersComponent implements OnInit {
  suppliers: Supplier[] = [];

  currentSupplier: Supplier = {} as Supplier;

  showForm = false;
  editMode = false;

  userId = Number(localStorage.getItem('userId'));

  constructor(private supplierService: SupplierService) {}

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.supplierService.getSuppliers().subscribe(data => 
      this.suppliers = data);
  }

  openForm(supplier?: Supplier): void {
    this.showForm = true;
    this.currentSupplier = supplier ? { ...supplier } : {} as Supplier;
  }

  closeForm(): void {
    this.showForm = false;
  }

  saveSupplier(supplier: Supplier) {
    this.supplierService.createSupplier(supplier, this.userId).subscribe(() => {
      this.loadSuppliers()
      this.closeForm();
    })
  }

  disableSupplier(supplier: Supplier) {
    this.supplierService.deactivateSupplier(supplier.id, 'Admin')
      .subscribe(() => supplier.status = 0);
  }

  enableSupplier(supplier: Supplier) {
    this.supplierService.activateSupplier(supplier.id, 'Admin')
      .subscribe(() => supplier.status = 1);
  }
}
