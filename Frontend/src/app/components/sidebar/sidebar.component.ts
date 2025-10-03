import { Component, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { HttpBackend } from '@angular/common/http';

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
  standalone: true
})
export class SidebarComponent {

  private readonly BREAKPOINT = 768;

  navItems = [
    { name: 'Productos', icon: 'square', route: '/products' },
    { name: 'Proveedores', icon: 'people', route: '/suppliers' },
    { name: 'Compras', icon: 'shopping_cart', route: '/purchases' },
    { name: 'Reportes', icon: 'article', route: '/reports' },
    { name: 'Usuario', icon: 'person', route: '/user-management' },
  ];
  
  constructor(private router: Router) {  }

  isRouteActive(route: string): boolean {
    return this.router.url === route;
  }

  navigateTo(route: string) {
    if (route === '#') return;
    
    this.router.navigate([route]);
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
