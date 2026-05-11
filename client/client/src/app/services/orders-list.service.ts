import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../enviorments/enviorment';
import { Order } from '../models/Order';

@Injectable({
  providedIn: 'root',
})
export class OrdersListService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl + '/api/order';

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // קבלת כל ההזמנות
  getAllOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(this.apiUrl+'/all', { headers: this.getHeaders() });
  }
  getAllOrdersByUserId(userId: number): Observable<Order[]> {
    return this.http.get<Order[]>(this.apiUrl+'/user/'+userId, { headers: this.getHeaders() });
  }
}
