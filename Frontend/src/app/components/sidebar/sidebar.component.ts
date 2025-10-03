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
  isSidebarCollapsed = signal(false); 
  isMobile = signal(false); 
  isSidebarOpen = signal(false); 

  private readonly BREAKPOINT = 768;

  navItems = [
    { name: 'Productos', icon: 'square', route: '/products' },
    { name: 'Proveedores', icon: 'people', route: '/suppliers' },
    { name: 'Compras', icon: 'shopping_cart', route: '/purchases' },
    { name: 'Reportes', icon: 'article', route: '/reports' },
    { name: 'Usuario', icon: 'person', route: '/user-management' },
  ];
  
  constructor(private router: Router) {
    if (typeof window !== 'undefined') {
      this.checkWindowSize();
    }
  }

  @HostListener('window:resize')
  onResize() {
    this.checkWindowSize();
  }

  private checkWindowSize() {
    if (typeof window === 'undefined') return;
    
    const isCurrentlyMobile = window.innerWidth <= this.BREAKPOINT;
    this.isMobile.set(isCurrentlyMobile);

    if (!isCurrentlyMobile) {
      this.isSidebarOpen.set(false);
    }
  }

  toggleSidebar(event: MouseEvent) {
    event.stopPropagation();
    
    if (this.isMobile()) {
      this.isSidebarOpen.update(value => !value);
    } else {
      this.isSidebarCollapsed.update(value => !value);
    }
  }
  
  closeSidebarOnMobile(event: MouseEvent) {
    if (this.isMobile() && this.isSidebarOpen()) {
      this.isSidebarOpen.set(false);
    }
  }

  isRouteActive(route: string): boolean {
    return this.router.url === route;
  }

  navigateTo(route: string) {
    if (route === '#') return;
    
    this.router.navigate([route]);
    
    if (this.isMobile()) {
      this.isSidebarOpen.set(false);
    }
  }

  getSidebarClasses(): string {
    let classes = 'sidebar';
    
    if (this.isMobile()) {
      classes += this.isSidebarOpen() ? ' mobile-open' : '';
    } else {
      classes += this.isSidebarCollapsed() ? ' sidebar-collapsed' : ' sidebar-expanded';
    }
    
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
