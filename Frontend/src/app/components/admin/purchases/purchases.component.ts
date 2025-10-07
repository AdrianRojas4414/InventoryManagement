import { Component, OnInit } from '@angular/core';
import { SidebarComponent } from '../../sidebar/sidebar.component';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SupplierService } from '../../../services/supplier.service';
import { ProductService } from '../../../services/product.service';

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

interface Supplier {
  id: number;
  name: string;
  nit: string;
  address?: string;
  phone?: string;
  email?: string;
  contactName?: string;
  status?: number;
  showOptions?: boolean;
  disabled?: boolean;
}

interface Product {
  id: number;
  name: string;
  description: string;
  categoryId: number;
  totalStock: number;
  status?: number;
}

@Component({
  selector: 'app-purchases',
  imports: [ReactiveFormsModule, SidebarComponent, CommonModule],
  templateUrl: './purchases.component.html',
  styleUrl: './purchases.component.css'
})
export class PurchasesComponent implements OnInit {
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
        { producto: 'Aceite 1L', cantidad: 5, precioUnitario: 18 }
      ]
    }
  ];

  showPurchaseForm = false;
  purchaseForm!: FormGroup;
  
  // Datos para los dropdowns
  suppliers: Supplier[] = [];
  products: Product[] = [];
  filteredProducts: Product[] = [];
  
  // Control de dropdowns abiertos
  showSupplierDropdown = false;
  supplierSearchTerm = '';
  showProductDropdown = false;
  productSearchTerm = '';
  activeProductDropdown: number | null = null;
  productSearchTerms: { [key: number]: string } = {};

  constructor(
    private fb: FormBuilder,
    private supplierService: SupplierService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadSuppliers();
    this.loadProducts();
  }

  initForm(): void {
    const today = new Date().toISOString().split('T')[0];
    
    this.purchaseForm = this.fb.group({
      supplierId: ['', Validators.required],
      date: [today, Validators.required],
      details: this.fb.array([])
    });
  }

  get details(): FormArray {
    return this.purchaseForm.get('details') as FormArray;
  }

  loadSuppliers(): void {
      this.supplierService.getSuppliers().subscribe(
        (data: Supplier[]) => { this.suppliers = data; });
    }

 loadProducts(): void {
    this.productService.getProducts().subscribe(products => {
      this.products = products;
      this.filteredProducts = products;
    }, error => {
      console.error('Error cargando productos:', error);
    });
  }

  createDetailRow(productId?: number, productName?: string): FormGroup {
    return this.fb.group({
      productId: [productId || '', Validators.required],
      productName: [productName || ''],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  addDetailRow(): void {
    this.details.push(this.createDetailRow());
  }

  removeDetailRow(index: number): void {
    this.details.removeAt(index);
  }

  incrementQuantity(index: number): void {
    const control = this.details.at(index).get('quantity');
    if (control) {
      control.setValue(control.value + 1);
    }
  }

  decrementQuantity(index: number): void {
    const control = this.details.at(index).get('quantity');
    if (control && control.value > 1) {
      control.setValue(control.value - 1);
    }
  }

  getRowSubtotal(index: number): number {
    const detail = this.details.at(index).value;
    return (detail.quantity || 0) * (detail.unitPrice || 0);
  }

  getTotalAmount(): number {
    let total = 0;
    for (let i = 0; i < this.details.length; i++) {
      total += this.getRowSubtotal(i);
    }
    return total;
  }

  // Manejo de dropdown de proveedores
  toggleSupplierDropdown(): void {
    this.showSupplierDropdown = !this.showSupplierDropdown;
    if (this.showSupplierDropdown) {
      this.supplierSearchTerm = '';
    }
  }

  selectSupplier(supplier: Supplier): void {
    this.purchaseForm.patchValue({ supplierId: supplier.id });
    this.showSupplierDropdown = false;
  }

  getSelectedSupplierName(): string {
    const supplierId = this.purchaseForm.get('supplierId')?.value;
    const supplier = this.suppliers.find(s => s.id === supplierId);
    return supplier ? supplier.name : 'Seleccione un proveedor';
  }

  getFilteredSuppliers(): Supplier[] {
    if (!this.supplierSearchTerm) {
      return this.suppliers;
    }
    return this.suppliers.filter(s => 
      s.name.toLowerCase().includes(this.supplierSearchTerm.toLowerCase())
    );
  }

  // Manejo de dropdown de productos
  toggleProductDropdown(
    index: number
  ): void {
    this.showProductDropdown = !this.showProductDropdown;
    this.activeProductDropdown = index
    if (this.showProductDropdown) {
      this.productSearchTerm = '';
    }
    // if (this.activeProductDropdown === index) {
    //   this.activeProductDropdown = null;
    // } else {
    //   this.activeProductDropdown = index;
    //   this.productSearchTerms[index] = '';
    // }
    console.log("Toggle product", this.productSearchTerm)
  }

  selectProduct(product: Product, index: number): void {
    // Verificar si el producto ya está seleccionado
    const alreadySelected = this.details.controls.some((control, i) => 
      i !== index && control.get('productId')?.value === product.id
    );

    if (alreadySelected) {
      alert('Este producto ya está agregado en otra fila');
      return;
    }

    this.details.at(index).patchValue({
      productId: product.id,
      productName: product.name
    });
    this.activeProductDropdown = null;
  }

  getSelectedProductName(index: number): string {
    const productId = this.details.at(index).get('productId')?.value;
    const product = this.products.find(p => p.id === productId);
    return product ? product.name : 'Seleccione un producto';
  }

  getFilteredProducts(index: number): Product[] {
    if (!this.productSearchTerm) {
      return this.products;
    }
    return this.products.filter(p => 
      p.name.toLowerCase().includes(this.productSearchTerm.toLowerCase())
    );
    // const searchTerm = this.productSearchTerms[index] || '';
    
    // // Filtrar productos que ya están seleccionados
    // const selectedProductIds = this.details.controls
    //   .map(control => control.get('productId')?.value)
    //   .filter(id => id);

    // let filtered = this.products.filter(p => !selectedProductIds.includes(p.id));

    // if (searchTerm) {
    //   filtered = filtered.filter(p => 
    //     p.name.toLowerCase().includes(searchTerm.toLowerCase())
    //   );
    // }
    // //console.log(filtered)
    // return filtered;
  }

  // Manejo del formulario
  openPurchaseProductForm(): void {
    this.showPurchaseForm = true;
    this.initForm();
  }

  closePurchaseForm(): void {
    this.showPurchaseForm = false;
    this.activeProductDropdown = null;
    this.showSupplierDropdown = false;
  }

  onSubmit(): void {
    // Validar que haya al menos un producto
    if (this.details.length === 0) {
      alert('Debe agregar al menos un producto');
      return;
    }

    // Validar que todos los productos estén completos
    if (this.purchaseForm.invalid) {
      alert('Por favor complete todos los campos requeridos correctamente');
      this.purchaseForm.markAllAsTouched();
      return;
    }

    // Validar que no haya filas con producto sin seleccionar
    const hasEmptyProduct = this.details.controls.some(control => 
      !control.get('productId')?.value
    );

    if (hasEmptyProduct) {
      alert('Debe seleccionar un producto en todas las filas o eliminar las filas vacías');
      return;
    }

    console.log('Formulario válido:', this.purchaseForm.value);
    console.log('Total:', this.getTotalAmount());
    
    // TODO: Aquí enviarías los datos a tu servicio
    // this.purchaseService.createPurchase(this.purchaseForm.value).subscribe(...)
    
    // Cerrar el formulario después de guardar
    this.closePurchaseForm();
  }

  // Para la tabla de compras existente
  toggleDetails(purchase: Purchase): void {
    purchase.expanded = !purchase.expanded;
  }

  getSubtotal(detalle: PurchaseDetail): number {
    return detalle.cantidad * detalle.precioUnitario;
  }
}