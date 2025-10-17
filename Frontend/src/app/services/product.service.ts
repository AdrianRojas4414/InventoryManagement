import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config';

export interface Product {
  id: number;
  name: string;
  description: string;
  categoryId: number;
  totalStock: number;
  status?: number;
  showOptions?: boolean;
}

export interface CreateProductDto {
  name: string;
  description: string;
  categoryId: number;
  totalStock: number;
}

@Injectable({
  providedIn: 'root'
})

export class ProductService {
  private apiUrl = `${API_URL}/products`;

  constructor(private http: HttpClient) {}

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  createProduct(product: CreateProductDto, userId: number): Observable<Product> {
    const headers = new HttpHeaders().set('userId', userId.toString());
    return this.http.post<Product>(this.apiUrl, product, { headers });
  }

  updateProduct(id: number, product: CreateProductDto): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/${id}`, product);
  }

  desactivate(id: number, userRole: string): Observable<any> {
    const headers = new HttpHeaders().set('userRole', userRole);
    return this.http.delete(`${this.apiUrl}/${id}`, { headers, responseType: 'text' });
  }

  activate(id: number, userRole: string): Observable<string> {
    const headers = new HttpHeaders().set('userRole', userRole);
    return this.http.put(`${this.apiUrl}/${id}/activate`, null, { headers, responseType: 'text' });
  }
}
