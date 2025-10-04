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
}

export interface CreateProductDto {
  id: number;
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
}
