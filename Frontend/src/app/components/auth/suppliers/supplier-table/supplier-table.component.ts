import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Supplier } from '../../../../services/supplier.service';

@Component({
  selector: 'app-supplier-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './supplier-table.component.html',
  styleUrls: ['./supplier-table.component.css']
})
export class SupplierTableComponent {
  @Input() suppliers: Supplier[] = [];
  @Input() isAdmin = true;

  @Output() edit = new EventEmitter<Supplier>();
  @Output() disable = new EventEmitter<Supplier>();
  @Output() enable = new EventEmitter<Supplier>();

  toggleOptions(supplier: Supplier) {
    this.suppliers.forEach(s => {
      if (s !== supplier) s.showOptions = false;
    });
    supplier.showOptions = !supplier.showOptions;
  }
}
