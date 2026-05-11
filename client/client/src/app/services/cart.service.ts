import { environment } from '../enviorments/enviorment';
import { CartItem } from '../models/cartItem';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { AuthService } from './auth.service';


@Injectable({
  providedIn: 'root',
})
export class CartService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl + '/api/cart';
  private authService = inject(AuthService);

  private cartItemsSubject = new BehaviorSubject<CartItem[]>([]);
  cartItems$ = this.cartItemsSubject.asObservable();

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }


  getCartItems(userId: number): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(`${this.apiUrl}/GetCartByUserId?userId=${userId}`,{ headers: this.getHeaders() });
  }

  // Refresh the internal cart items subject from server using current user id
  refreshCart(): void {
    const userId = this.authService.getUserId();
    if (userId == null) {
      this.cartItemsSubject.next([]);
      return;
    }
    const idNum = typeof userId === 'string' ? Number(userId) : userId;
    console.log('CartService: refreshing cart for userId', idNum);
    this.getCartItems(idNum).subscribe(items => this.cartItemsSubject.next(items || []), (err) => {
      console.error('CartService: refresh failed', err);
      this.cartItemsSubject.next([]);
    });
  }

  removeItem(itemId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${itemId}`, { 
      headers: this.getHeaders(),
      responseType: 'text' 
    }).pipe(tap(() => this.refreshCart()));
  }


  updateCartItemQuantity(cartItemId: number, diff: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${cartItemId}?addCount=${diff}`, {}, { 
      headers: this.getHeaders(),
      responseType: 'text' 
    }).pipe(tap(() => this.refreshCart()));
  }
  buyCart(userId: number) {
    const params = new HttpParams().set('userId', userId.toString());
    
    return this.http.post(
      `${environment.apiUrl}/api/Order/buy`, 
      {}, 
      { 
        params: params,
        headers: this.getHeaders(), 
        responseType: 'text' 
      }
    ).pipe(tap(() => this.refreshCart()));
  }
}
