import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config';

export interface User {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
  secondLastName?: string;
  role: string;
}

export interface CreateUserDto {
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  secondLastName?: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class UsersService {
  private base = `${API_URL}/users`;

  constructor(private http: HttpClient) {}

  private adminHeaders(): HttpHeaders {
    const role = localStorage.getItem('role') || '';
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'userRole': role
    });
  }

  list(): Observable<User[]> {
    return this.http.get<User[]>(this.base, { withCredentials: true });
  }

  create(dto: CreateUserDto): Observable<User> {
    return this.http.post<User>(this.base, dto, {
      headers: this.adminHeaders(),
      withCredentials: true
    });
  }

  update(id: number, user: Partial<User>): Observable<User> {
    return this.http.put<User>(`${this.base}/${id}`, user, {
      withCredentials: true
    });
  }

  updateRole(id: number, role: string): Observable<void> {
    return this.http.put<void>(
      `${this.base}/${id}/role`,
      { role },
      { headers: this.adminHeaders(), withCredentials: true }
    );
  }

  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`, {
      withCredentials: true
    });
  }
}
