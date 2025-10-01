import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { UsersService, User, CreateUserDto } from '../../../services/users.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatInputModule, MatFormFieldModule],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent  implements OnInit {
  empleados: User[] = [];
  showForm = false;

  nuevo: CreateUserDto = {
    username: '',
    password: '',
    firstName: '',
    lastName: '',
    secondLastName: '',
    role: 'User'
  };

  constructor(private empleadosService: UsersService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.empleadosService.list().subscribe(data => (this.empleados = data));
  }

  toggleForm() {
    this.showForm = !this.showForm;
  }

  crear() {
    this.empleadosService.create(this.nuevo).subscribe(() => {
      this.load();
      this.nuevo = { username: '', password: '', firstName: '', lastName: '', secondLastName: '', role: 'User' };
      this.showForm = false;
    });
  }

  eliminar(id: number) {
    this.empleadosService.remove(id).subscribe(() => this.load());
  }

  cambiarRol(u: User, rol: string) {
    this.empleadosService.updateRole(u.id, rol).subscribe(() => this.load());
  }
}
