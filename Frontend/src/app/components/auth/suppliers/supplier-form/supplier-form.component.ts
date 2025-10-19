import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CreateSupplierDto, Supplier, SupplierService } from '../../../../services/supplier.service';

interface CountryCode {
  code: string;
  label: string;
}

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
  @Output() saved = new EventEmitter<Supplier>();

  editMode = false;
  userId = Number(localStorage.getItem('userId'));

  errorMessage = '';
  successMessage = '';

  // Campos auxiliares para el teléfono
  countryCode: string = '+591';
  phoneNumber: string = '';

  // Dropdown de países
  showCountryDropdown = false;
  countrySearchTerm = '';
  
  countries: CountryCode[] = [
    { code: '+591', label: '+591 (Bolivia)' },
    { code: '+54', label: '+54 (Argentina)' },
    { code: '+56', label: '+56 (Chile)' },
    { code: '+57', label: '+57 (Colombia)' },
    { code: '+51', label: '+51 (Perú)' },
    { code: '+52', label: '+52 (México)' }
  ];

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

  // Métodos para el manejo del dropdown de países
  toggleCountryDropdown(): void {
    this.showCountryDropdown = !this.showCountryDropdown;
    if (this.showCountryDropdown) {
      this.countrySearchTerm = '';
    }
  }

  getSelectedCountryCode(): string {
    const country = this.countries.find(c => c.code === this.countryCode);
    return country ? country.label : '+591 (Bolivia)';
  }

  getFilteredCountries(): CountryCode[] {
    if (!this.countrySearchTerm) {
      return this.countries;
    }
    return this.countries.filter(c => 
      c.label.toLowerCase().includes(this.countrySearchTerm.toLowerCase()) ||
      c.code.includes(this.countrySearchTerm)
    );
  }

  selectCountry(country: CountryCode): void {
    this.countryCode = country.code;
    this.showCountryDropdown = false;
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

    // Validar longitudes mínimas manualmente
    if (
      !this.supplier.name || this.supplier.name.length < 4 ||
      !this.supplier.nit || this.supplier.nit.length < 7 ||
      !this.phoneNumber || this.phoneNumber.length < 6 ||
      !this.supplier.email || this.supplier.email.length < 5 ||
      !this.supplier.contactName || this.supplier.contactName.length < 2 ||
      !this.supplier.address || this.supplier.address.length < 4
    ) {
      this.errorMessage = 'No se cumple con el tamaño mínimo requerido.';
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
      next: (createdSupplier: Supplier) => {
        this.successMessage = this.editMode
          ? 'Proveedor actualizado correctamente.'
          : 'Proveedor agregado correctamente.';

        setTimeout(() => {
          this.saved.emit(createdSupplier);
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
