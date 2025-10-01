import { Routes } from '@angular/router';
import { ProductsComponent } from './components/products/products.component';
import { SuppliersComponent } from './components/suppliers/suppliers.component';

export const routes: Routes = [
  { path: '', redirectTo: '/products', pathMatch: 'full' },
  { path: 'products', component: ProductsComponent },
  { path: 'suppliers', component: SuppliersComponent }
];
