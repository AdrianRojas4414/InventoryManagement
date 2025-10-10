import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { API_URL } from '../../../config';
import { SidebarComponent } from '../../sidebar/sidebar.component';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    SidebarComponent
  ],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: any[] = [];
  showForm = false;

  newUser = {
    username: '',
    password: '',
    firstName: '',
    lastName: '',
    secondLastName: '',
    role: 'User'
  };

  private baseUrl = `${API_URL}/users`;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  /** Cargar usuarios */
  loadUsers(): void {
    this.http.get<any[]>(this.baseUrl).subscribe({
      next: (data) => (this.users = data),
      error: (err) => console.error('Error cargando usuarios:', err)
    });
  }

  /** Crear usuario */
  createUser(): void {
    const headers = new HttpHeaders({
      userRole: localStorage.getItem('role') || ''
    });

    this.http.post(this.baseUrl, this.newUser, { headers }).subscribe({
      next: () => {
        this.loadUsers();
        this.toggleForm();
        this.resetForm();
      },
      error: (err) => {
        console.error('Error al crear usuario:', err);
        alert('Solo un administrador puede crear usuarios.');
      }
    });
  }

  /** Eliminar usuario (dar de baja) */
  deleteUser(id: number): void {
    if (confirm('¿Seguro que deseas eliminar este usuario?')) {
      const headers = new HttpHeaders({
        userRole: localStorage.getItem('role') || ''
      });

      this.http.delete(`${this.baseUrl}/${id}`, { headers }).subscribe({
        next: () => this.loadUsers(),
        error: (err) => {
          console.error('Error al eliminar usuario:', err);
          alert('Solo un administrador puede eliminar usuarios.');
        }
      });
    }
  }

  /** Cambiar rol del usuario */
  updateRole(user: any, role: string): void {
    const headers = new HttpHeaders({
      userRole: localStorage.getItem('role') || ''
    });

    // backend espera un string plano, no un objeto
    this.http.put(`${this.baseUrl}/${user.id}/role`, `"${role}"`, { headers }).subscribe({
      next: () => this.loadUsers(),
      error: (err) => {
        console.error('Error al cambiar rol:', err);
        alert('Solo un administrador puede cambiar roles.');
      }
    });
  }

  /** Mostrar/ocultar formulario */
  toggleForm(): void {
    this.showForm = !this.showForm;
  }

  /** Resetear formulario */
  resetForm(): void {
    this.newUser = {
      username: '',
      password: '',
      firstName: '',
      lastName: '',
      secondLastName: '',
      role: 'User'
    };
  }
}
