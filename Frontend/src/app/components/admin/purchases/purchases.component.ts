import { Component } from '@angular/core';
import { SidebarComponent } from '../../sidebar/sidebar.component';
import { CommonModule } from '@angular/common';

interface PurchaseDetail {
  producto: string;
  cantidad: number;
  precioUnitario: number;
}

interface Purchase {
  id: number;
  fecha: string;
  proveedor: string;
  total: string;
  detalles: PurchaseDetail[];
  expanded?: boolean;
}

@Component({
  selector: 'app-purchases',
  imports: [SidebarComponent, CommonModule],
  templateUrl: './purchases.component.html',
  styleUrl: './purchases.component.css'
})
export class PurchasesComponent {
  purchases: Purchase[] = [
    {
      id: 0,
      fecha: '14/09/2025',
      proveedor: 'MILCAR',
      total: '300 BOB',
      expanded: false,
      detalles: [
        { producto: 'Arroz 1kg', cantidad: 10, precioUnitario: 15 },
        { producto: 'Azúcar 1kg', cantidad: 8, precioUnitario: 12 },
        { producto: 'Aceite 1L', cantidad: 5, precioUnitario: 18 },
        { producto: 'Aceite 1L', cantidad: 5, precioUnitario: 18 },
        { producto: 'Aceite 1L', cantidad: 5, precioUnitario: 18 },
        { producto: 'Aceite 1L', cantidad: 5, precioUnitario: 18 }
      ]
    },
    {
      id: 1,
      fecha: '15/09/2025',
      proveedor: 'DISTRIBUIDORA XYZ',
      total: '450 BOB',
      expanded: false,
      detalles: [
        { producto: 'Harina 1kg', cantidad: 15, precioUnitario: 10 },
        { producto: 'Sal 1kg', cantidad: 20, precioUnitario: 5 },
        { producto: 'Fideos 500g', cantidad: 12, precioUnitario: 8 }
      ]
    },
    {
      id: 2,
      fecha: '15/09/2025',
      proveedor: 'DISTRIBUIDORA XYZ',
      total: '450 BOB',
      expanded: false,
      detalles: [
        { producto: 'Harina 1kg', cantidad: 15, precioUnitario: 10 },
        { producto: 'Sal 1kg', cantidad: 20, precioUnitario: 5 },
        { producto: 'Fideos 500g', cantidad: 12, precioUnitario: 8 }
      ]
    },
    {
      id: 3,
      fecha: '15/09/2025',
      proveedor: 'DISTRIBUIDORA XYZ',
      total: '450 BOB',
      expanded: false,
      detalles: [
        { producto: 'Harina 1kg', cantidad: 15, precioUnitario: 10 },
        { producto: 'Sal 1kg', cantidad: 20, precioUnitario: 5 },
        { producto: 'Fideos 500g', cantidad: 12, precioUnitario: 8 }
      ]
    },
    {
      id: 4,
      fecha: '15/09/2025',
      proveedor: 'DISTRIBUIDORA XYZ',
      total: '450 BOB',
      expanded: false,
      detalles: [
        { producto: 'Harina 1kg', cantidad: 15, precioUnitario: 10 },
        { producto: 'Sal 1kg', cantidad: 20, precioUnitario: 5 },
        { producto: 'Fideos 500g', cantidad: 12, precioUnitario: 8 }
      ]
    },
    {
      id: 4,
      fecha: '15/09/2025',
      proveedor: 'DISTRIBUIDORA XYZ',
      total: '450 BOB',
      expanded: false,
      detalles: [
        { producto: 'Harina 1kg', cantidad: 15, precioUnitario: 10 },
        { producto: 'Sal 1kg', cantidad: 20, precioUnitario: 5 },
        { producto: 'Fideos 500g', cantidad: 12, precioUnitario: 8 }
      ]
    },
    {
      id: 4,
      fecha: '15/09/2025',
      proveedor: 'DISTRIBUIDORA XYZ',
      total: '450 BOB',
      expanded: false,
      detalles: [
        { producto: 'Harina 1kg', cantidad: 15, precioUnitario: 10 },
        { producto: 'Sal 1kg', cantidad: 20, precioUnitario: 5 },
        { producto: 'Fideos 500g', cantidad: 12, precioUnitario: 8 }
      ]
    },
    {
      id: 4,
      fecha: '15/09/2025',
      proveedor: 'DISTRIBUIDORA XYZ',
      total: '450 BOB',
      expanded: false,
      detalles: [
        { producto: 'Harina 1kg', cantidad: 15, precioUnitario: 10 },
        { producto: 'Sal 1kg', cantidad: 20, precioUnitario: 5 },
        { producto: 'Fideos 500g', cantidad: 12, precioUnitario: 8 }
      ]
    },
    {
      id: 4,
      fecha: '15/09/2025',
      proveedor: 'DISTRIBUIDORA XYZ',
      total: '450 BOB',
      expanded: false,
      detalles: [
        { producto: 'Harina 1kg', cantidad: 15, precioUnitario: 10 },
        { producto: 'Sal 1kg', cantidad: 20, precioUnitario: 5 },
        { producto: 'Fideos 500g', cantidad: 12, precioUnitario: 8 }
      ]
    }
  ];

  toggleDetails(purchase: Purchase): void {
    purchase.expanded = !purchase.expanded;
  }

  getSubtotal(detalle: PurchaseDetail): number {
    return detalle.cantidad * detalle.precioUnitario;
  }
}