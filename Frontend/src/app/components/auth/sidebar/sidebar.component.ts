import { Component, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { HttpBackend } from '@angular/common/http';
import { ConfirmModalComponent } from '../confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule, RouterModule, ConfirmModalComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
  standalone: true
})
export class SidebarComponent {

  private readonly BREAKPOINT = 768;
  showLogoutModal = false;

  navItems = [
    { name: 'Productos', icon: 'square', route: '/products' },
    { name: 'Proveedores', icon: 'people', route: '/suppliers' },
    { name: 'Compras', icon: 'shopping_cart', route: '/purchases' },
    { name: 'Reportes', icon: 'article', route: '/reports' },
    { name: 'Usuario', icon: 'person', route: '/user-management' },
    { name: 'Salir', icon: 'logout', route: 'logout'}
  ];
  
  constructor(private router: Router) {  }

  isRouteActive(route: string): boolean {
    return this.router.url === route;
  }

  navigateTo(route: string) {
    if (route === '#') return;
    if (route === 'logout') {
      this.showLogoutModal = true; // Muestra el modal
      return;
    }
    this.router.navigate([route]);
  }

  cancelLogout() {
    this.showLogoutModal = false;
  }

  confirmLogout() {
    this.showLogoutModal = false;
    localStorage.clear(); // Limpia datos del usuario
    sessionStorage.clear();
    this.router.navigate(['/login']); // Redirige al login
  }

  getSidebarClasses(): string {
    let classes = 'sidebar';
    return classes;
  }
  
  getNavItemClasses(route: string): string {
    const isActive = this.isRouteActive(route);
    let classes = 'nav-item';
    
    if (isActive) {
      classes += ' active';
    }
    
    return classes;
  }
}
