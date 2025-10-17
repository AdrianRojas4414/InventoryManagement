import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Category, CategoryService } from '../../../../../services/category.service';

@Component({
  selector: 'app-category-form',
  templateUrl: './category-form.component.html',
  styleUrls: ['./category-form.component.css'],
  imports: [FormsModule, CommonModule],
  standalone: true
})
export class CategoryFormComponent implements OnInit {
  @Input() category: Category = { name: '', description: '' };
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  editMode = false;
  userId = Number(localStorage.getItem('userId'));

  errorMessage: string = '';
  successMessage: string = '';

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.editMode = !!this.category.id;
  }

  save(form: NgForm): void {
    this.errorMessage = '';
    this.successMessage = '';

    this.category.description = this.category.description.trim();
    this.category.name = this.category.name.trim();

    const request = this.editMode
      ? this.categoryService.update(this.category.id!, this.category)
      : this.categoryService.addCategory(this.category, this.userId);

    request.subscribe({
      next: () => {
        this.successMessage = this.editMode
          ? 'Categoría actualizada exitosamente.'
          : 'Categoría creada exitosamente.';

        setTimeout(() => {
          this.saved.emit();
          this.close.emit();
        }, 1000);
      },
      error: (error) => {
        console.error('Error en la operación:', error);
        this.errorMessage = 'Ocurrió un error al guardar la categoría. Intenta nuevamente.';
      }
    });
  }

  cancel(): void {
    this.close.emit();
  }
}
