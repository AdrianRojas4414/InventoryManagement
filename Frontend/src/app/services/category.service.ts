import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config';

export interface Category {
  id: number;
  name: string;
  description: string;
  status?: number;
  showOptions?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = `${API_URL}/categories`;

  constructor(private http: HttpClient) { }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  addCategory(category: Category, userId: number): Observable<Category> {
    const headers = new HttpHeaders().set('userId', userId.toString());
    return this.http.post<Category>(this.apiUrl, category, { headers });
  }

  update(id: number, category: Category): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/${id}`, category);
  }

  desactivate(id: number, userRole: string): Observable<string> {
    const headers = new HttpHeaders().set('userRole', userRole);
    return this.http.delete(`${this.apiUrl}/${id}`, { headers, responseType: 'text' });
  }

  activate(id: number, userRole: string): Observable<string> {
    const headers = new HttpHeaders().set('userRole', userRole);
    return this.http.put(`${this.apiUrl}/${id}/activate`, null, { headers, responseType: 'text' });
  }
}
