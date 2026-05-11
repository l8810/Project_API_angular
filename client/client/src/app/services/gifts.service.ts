import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Gift } from '../models/gift';
import { CartService } from './cart.service';

@Injectable({
  providedIn: 'root',
})
export class GiftsService {
  private http = inject(HttpClient);
  private cartService = inject(CartService);


  getAllGifts() :Observable<Gift[]>{
    try {
      const apiUrl = 'http://localhost:5186/api/Gift';
      return this.http.get<any[]>(apiUrl)
    }
    catch(error) {
      throw error;
    }
  }

  addToCart(giftId: number, userId: number, count: number): Observable<any> {
    const token = localStorage.getItem('auth_token');
    
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    
    const body = { giftId, userId, count }; 
    const apiUrl = 'http://localhost:5186/api/Cart';
    
    return this.http.post(apiUrl, body, { headers, responseType: 'text' }).pipe(
      tap(() => {
        console.log('GiftsService: addToCart succeeded, refreshing cart');
        this.cartService.refreshCart();
      })
    );
  }
}
