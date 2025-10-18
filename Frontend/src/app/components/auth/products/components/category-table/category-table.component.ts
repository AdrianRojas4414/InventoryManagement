import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../../services/auth.service';
import { Category } from '../../../../../services/category.service';

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
  @Input() categoriesPaged: Category[] = [];
  @Input() currentPage: number = 0;
  @Input() totalPages: number = 0;
  @Input() totalItems: number = 0;
  @Output() edit = new EventEmitter<Category>();
  @Output() disable = new EventEmitter<Category>();
  @Output() enable = new EventEmitter<Category>();
  @Output() pageChange = new EventEmitter<number>();

  isAdmin = false;
  
  ngOnInit() {
    this.isAdmin = this.authService.isAdmin();
  }

  toggleOptions(category: Category) {
    category['showOptions'] = !category['showOptions'];
  }

  nextPage() {
    if (this.currentPage < this.totalPages - 1) {
      this.pageChange.emit(this.currentPage + 1);
    }
  }

  prevPage() {
    if (this.currentPage > 0) {
      this.pageChange.emit(this.currentPage - 1);
    }
  }

  goToPage(page: number) {
    if (page >= 0 && page < this.totalPages) {
      this.pageChange.emit(page);
    }
  }
}
