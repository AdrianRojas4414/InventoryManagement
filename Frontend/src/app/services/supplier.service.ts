import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config';

export interface Supplier {
  id: number;
  name: string;
  nit: string;
  address?: string;
  phone?: string;
  email?: string;
  contactName?: string;
  status?: number;

  showOptions?: boolean;
  disabled?: boolean;
}

@Injectable({
  providedIn: 'root'
})

export class SupplierService {
  private apiUrl = `${API_URL}/suppliers`;

  constructor(private http: HttpClient) {}

  getSuppliers(): Observable<Supplier[]> {
    return this.http.get<Supplier[]>(this.apiUrl);
  }

  createSupplier(supplier: Partial<Supplier>, userId: number): Observable<Supplier> {
    const headers = new HttpHeaders().set('userId', userId.toString());
    return this.http.post<Supplier>(this.apiUrl, supplier, { headers });
  }

  updateSupplier(id: number, supplier: Partial<Supplier>): Observable<Supplier> {
    return this.http.put<Supplier>(`${this.apiUrl}/${id}`, supplier);
  }

  deactivateSupplier(id: number, userRole: string): Observable<string> {
    const headers = new HttpHeaders().set('userRole', userRole);
    return this.http.delete(`${this.apiUrl}/${id}`, { headers, responseType: 'text' });
  }

  activateSupplier(id: number, userRole: string): Observable<string> {
  const headers = new HttpHeaders().set('userRole', userRole);
    return this.http.put(`${this.apiUrl}/${id}/activate`, {}, { headers, responseType: 'text' });
  }
}