import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config';

export interface PurchaseDetail {
  producto: string;
  cantidad: number;
  precioUnitario: number;
}

export interface Purchase {
  id: number;
  fecha: string;
  proveedor: string;
  total: string;
  expanded: boolean;
  detalles: PurchaseDetail[];
}

export interface CreatePurchaseDetail {
  productId: number;
  quantity: number;
  unitPrice: number;
}

export interface CreatePurchase {
  supplierId: number;
  purchaseDetails: CreatePurchaseDetail[];
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {
  private apiUrl = `${API_URL}/purchases`;

  constructor(private http: HttpClient) { }

  /**
   * Obtener todas las compras (Solo Admin)
   * @param userRole Rol del usuario (debe ser "Admin")
   */
  getAllPurchases(userRole: string = 'Admin'): Observable<Purchase[]> {
    const headers = new HttpHeaders({
      'userRole': userRole
    });

    return this.http.get<Purchase[]>(this.apiUrl, { headers });
  }

  /**
   * Obtener compras del usuario autenticado
   * @param userId ID del usuario
   */
  getUserPurchases(userId: number): Observable<Purchase[]> {
    const headers = new HttpHeaders({
      'userId': userId.toString()
    });

    return this.http.get<Purchase[]>(`${this.apiUrl}/user`, { headers });
  }

  /**
   * Obtener una compra específica por ID
   * @param purchaseId ID de la compra
   * @param userId ID del usuario
   * @param userRole Rol del usuario
   */
  getPurchaseById(purchaseId: number, userId: number, userRole: string): Observable<Purchase> {
    const headers = new HttpHeaders({
      'userId': userId.toString(),
      'userRole': userRole
    });

    return this.http.get<Purchase>(`${this.apiUrl}/${purchaseId}`, { headers });
  }

  /**
   * Crear una nueva compra
   * @param purchase Datos de la compra a crear
   * @param userId ID del usuario que crea la compra
   */
  createPurchase(purchase: CreatePurchase, userId: number): Observable<any> {
    const headers = new HttpHeaders({
      'userId': userId.toString(),
    });

    return this.http.post<any>(this.apiUrl, purchase, { headers });
  }

  /**
   * Actualizar una compra (Solo Admin)
   * @param purchaseId ID de la compra a actualizar
   * @param userRole Rol del usuario (debe ser "Admin")
   */
  updatePurchase(purchaseId: number, userRole: string = 'Admin'): Observable<any> {
    const headers = new HttpHeaders({
      'userRole': userRole
    });

    return this.http.put<any>(`${this.apiUrl}/${purchaseId}`, {}, { headers });
  }
}