import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { API_URL } from '../config';

interface LoginResponse {
  id: number;
  username: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = `${API_URL}/auth`;

  constructor(private http: HttpClient) {}

  async login(username: string, password: string): Promise<LoginResponse> {
    const resp = await firstValueFrom(
      this.http.post<LoginResponse>(
        `${this.base}/login`,
        { username, password },
        { withCredentials: true }
      )
    );
    localStorage.setItem('userId', String(resp.id));
    localStorage.setItem('role', resp.role);
    localStorage.setItem('username', resp.username);
    return resp;
  }

  logout(): void {
    localStorage.clear();
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('userId');
  }

  isAdmin(): boolean {
    return localStorage.getItem('role') === 'Admin';
  } 
}

