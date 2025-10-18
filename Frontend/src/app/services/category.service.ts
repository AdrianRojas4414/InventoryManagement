import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config';

export interface Category {
  id?: number;
  name: string;
  description: string;
  status?: number;
  showOptions?: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = `${API_URL}/categories`;

  constructor(private http: HttpClient) { }

  //METODO CON PAGINACION
  getCategories(page: number = 0, pageSize: number = 5): Observable<PaginatedResponse<Category>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<PaginatedResponse<Category>>(this.apiUrl, { params });
  }

  //METODO SIN PAGINACION
  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/all`);
  }

  addCategory(category: Category, userId: number): Observable<Category> {
    const headers = new HttpHeaders().set('userId', userId.toString());
    return this.http.post<Category>(this.apiUrl, category, { headers });
  }

  update(id: number, category: Category): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/${id}`, category);
  }
}
