import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { UsersService, User, CreateUserDto } from '../../../services/users.service';
import { SidebarComponent } from '../../sidebar/sidebar.component';
@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatIconModule,
    MatSelectModule,
    SidebarComponent
  ],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  showForm = false;

  newUser: CreateUserDto = {
    username: '',
    password: '',
    firstName: '',
    lastName: '',
    secondLastName: '',
    role: 'User'
  };

  constructor(private usersService: UsersService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.usersService.list().subscribe(data => (this.users = data));
  }

  toggleForm() {
    this.showForm = !this.showForm;
  }

  createUser() {
    this.usersService.create(this.newUser).subscribe(() => {
      this.loadUsers();
      this.newUser = { username: '', password: '', firstName: '', lastName: '', secondLastName: '', role: 'User' };
      this.showForm = false;
    });
  }

  deleteUser(id: number) {
    this.usersService.remove(id).subscribe(() => this.loadUsers());
  }

  updateRole(user: User, role: string) {
    this.usersService.updateRole(user.id, role).subscribe(() => this.loadUsers());
  }
}
