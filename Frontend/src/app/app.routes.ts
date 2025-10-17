import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { UserManagementComponent } from './components/admin/user-management/user-management.component';
import { ProductsComponent } from './components/auth/products/products.component';
import { SuppliersComponent } from './components/auth/suppliers/suppliers.component';
import { PurchasesComponent } from './components/auth/purchases/purchases.component';
import { AdminGuard } from './guards/admin.guard';
import { AuthGuard } from './guards/auth.guard';


export const routes: Routes = [
  { path: 'login', component: LoginComponent },

  { path: 'products', component: ProductsComponent, canActivate: [AuthGuard]},
  { path: 'suppliers', component: SuppliersComponent, canActivate: [AuthGuard]},
  { path: 'purchases', component: PurchasesComponent, canActivate: [AuthGuard]},
  { path: 'user-management', component: UserManagementComponent, canActivate: [AdminGuard] },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];
