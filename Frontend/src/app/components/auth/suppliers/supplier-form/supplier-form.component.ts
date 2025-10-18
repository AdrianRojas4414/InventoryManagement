import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CreateSupplierDto, Supplier, SupplierService } from '../../../../services/supplier.service';

@Component({
  selector: 'app-supplier-form',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './supplier-form.component.html',
  styleUrls: ['./supplier-form.component.css']
})
export class SupplierFormComponent implements OnInit {
  @Input() supplier: Supplier = {} as Supplier;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  editMode = false;
  userId = Number(localStorage.getItem('userId'));

  errorMessage = '';
  successMessage = '';

  // Campos auxiliares para el teléfono
  countryCode: string = '+591';
  phoneNumber: string = '';

  constructor(private supplierService: SupplierService) {}

  ngOnInit(): void {
    this.editMode = !!this.supplier.id;

    // Si el proveedor ya tiene teléfono, separa código y número
    if (this.supplier.phone) {
      const parts = this.supplier.phone.split(' ');
      if (parts.length === 2) {
        this.countryCode = parts[0];
        this.phoneNumber = parts[1];
      } else {
        this.phoneNumber = this.supplier.phone;
      }
    } else {
      this.phoneNumber = '';
    }
  }

  save(): void {
    // Limpiar espacios
    this.supplier.name = this.supplier.name?.trim();
    this.supplier.contactName = this.supplier.contactName?.trim();
    this.supplier.address = this.supplier.address?.trim();
    this.supplier.email = this.supplier.email?.trim();

    // Validaciones básicas
    if (!this.supplier.name || /^\d+$/.test(this.supplier.name)) {
      this.errorMessage = 'El nombre no puede ser solo números.';
      return;
    }

    // Unir el código y el número en un solo string
    const fullPhone = `${this.countryCode} ${this.phoneNumber}`;

    const supplierData: CreateSupplierDto = {
      name: this.supplier.name,
      nit: this.supplier.nit,
      phone: fullPhone,
      email: this.supplier.email,
      contactName: this.supplier.contactName,
      address: this.supplier.address,
    };

    const request = this.editMode
      ? this.supplierService.updateSupplier(this.supplier.id, supplierData)
      : this.supplierService.createSupplier(supplierData, this.userId);

    request.subscribe({
      next: () => {
        this.successMessage = this.editMode
          ? 'Proveedor actualizado correctamente.'
          : 'Proveedor agregado correctamente.';

        setTimeout(() => {
          this.saved.emit();
          this.close.emit();
        }, 1000);
      },
      error: (error: any) => {
        console.error('Error al guardar proveedor:', error);

        // Si el backend envía un mensaje específico
        if (error.status === 400 && error.error) {
          this.errorMessage = error.error; // Mensaje del backend
        } else if (error.status === 500) {
          this.errorMessage = 'Error interno del servidor.';
        } else {
          this.errorMessage = 'Error al guardar el proveedor. Verifica los datos.';
        }
      }
    });
  }

  cancel(): void {
    this.close.emit();
  }
}
