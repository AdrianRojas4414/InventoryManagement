import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config';

export interface Category {
  id?: number;
  name: string;
  description: string;
  status?: number;
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
}
