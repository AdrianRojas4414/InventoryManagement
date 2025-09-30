import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductsComponent } from './components/products/products.component';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterModule, ProductsComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'inventory-system';
}
