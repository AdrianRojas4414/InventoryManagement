import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { UserManagementComponent } from './components/admin/user-management/user-management.component';
import { ProductsComponent } from './components/admin/products/products.component';
import { SuppliersComponent } from './components/admin/suppliers/suppliers.component';
import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';


export const routes: Routes = [
  { path: 'login', component: LoginComponent },

  { path: 'admin/products', component: ProductsComponent, canActivate: [AuthGuard, AdminGuard] },
  { path: 'admin/suppliers', component: SuppliersComponent, canActivate: [AuthGuard, AdminGuard] },
  { path: 'admin/user-management', component: UserManagementComponent, canActivate: [AuthGuard, AdminGuard] },

  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];
