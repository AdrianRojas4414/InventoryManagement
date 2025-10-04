import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { UserManagementComponent } from './components/admin/user-management/user-management.component';
import { ProductsComponent } from './components/admin/products/products.component';
import { SuppliersComponent } from './components/admin/suppliers/suppliers.component';
import { AuthGuard } from './guards/auth.guard';
import { PurchasesComponent } from './components/admin/purchases/purchases.component';


export const routes: Routes = [
  { path: 'login', component: LoginComponent },

  { path: 'products', component: ProductsComponent, canActivate: [AuthGuard] },
  { path: 'suppliers', component: SuppliersComponent, canActivate: [AuthGuard] },
  { path: 'purchases', component: PurchasesComponent, canActivate: [AuthGuard] },
  { path: 'user-management', component: UserManagementComponent, canActivate: [AuthGuard] },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];
