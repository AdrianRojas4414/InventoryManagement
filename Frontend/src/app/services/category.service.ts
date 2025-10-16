import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config';

export interface Category {
  id?: number;
  name: string;
  description: string;
  status?: number;
  showOptions?: number
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

  delete(id: number, userRole: string): Observable<any> {
    const headers = new HttpHeaders().set('userRole', userRole);
    return this.http.delete(`${this.apiUrl}/${id}`, { headers });
  }

  activate(id: number, userRole: string): Observable<any> {
    const headers = new HttpHeaders().set('userRole', userRole);
    return this.http.put(`${this.apiUrl}/${id}/activate`, null, { headers });
  }
}
