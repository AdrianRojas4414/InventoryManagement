import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CategoryService, Category } from '../../../../../services/category.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../../services/auth.service';

@Component({
  selector: 'app-category-table',
  templateUrl: './category-table.component.html',
  styleUrls: ['./category-table.component.css'],
  imports: [FormsModule, CommonModule],
})
export class CategoryTableComponent {
  Math = Math;
  constructor(private authService: AuthService) {} 
  @Input() categories: Category[] = [];
  @Output() edit = new EventEmitter<Category>();
  @Output() disable = new EventEmitter<Category>();
  @Output() enable = new EventEmitter<Category>();

  isAdmin = false;

  pageSize = 5;    
  currentPage = 0;   
  pagedCategories: Category[] = [];
  
  ngOnInit() {
    this.isAdmin = this.authService.isAdmin();
    this.updatePagedCategories();
  }

  ngOnChanges() {
    // cada vez que categories cambien, recalcula
    this.currentPage = 0;
    this.updatePagedCategories();
  }

  toggleOptions(category: Category) {
    category['showOptions'] = !category['showOptions'];
  }

  nextPage() {
    if ((this.currentPage + 1) * this.pageSize < this.categories.length) {
      this.currentPage++;
      this.updatePagedCategories();
    }
  }

  prevPage() {
    if (this.currentPage > 0) {
      this.currentPage--;
      this.updatePagedCategories();
    }
  }

  updatePagedCategories() {
    const start = this.currentPage * this.pageSize;
    const end = start + this.pageSize;
    this.pagedCategories = this.categories.slice(start, end);
  }
}
