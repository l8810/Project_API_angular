import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../enviorments/enviorment';
import { Gift } from '../models/gift';

@Injectable({
  providedIn: 'root',
})
export class GiftListService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl + '/api/gift';

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // קבלת כל המתנות
  getAllGifts(): Observable<Gift[]> {
    return this.http.get<Gift[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  // קבלת מתנה לפי ID
  getGiftById(id: number): Observable<Gift> {
    return this.http.get<Gift>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  // יצירת מתנה חדשה
  createGift(gift: Gift): Observable<Gift> {
    const createDto = {
      name: gift.name,
      description: gift.description,
      donorId: gift.donorId,
      price: gift.price,
      categoryId: gift.categoryId,
      picture: gift.picture
    };
    return this.http.post<Gift>(this.apiUrl, createDto, { headers: this.getHeaders() });
  }

  // עדכון מתנה קיימת
  updateGift(id: number, gift: Gift): Observable<Gift> {
    const updateDto = {
      name: gift.name,
      description: gift.description,
      donorId: gift.donorId,
      price: gift.price,
      categoryId: gift.categoryId,
      picture: gift.picture
    };
    return this.http.put<Gift>(`${this.apiUrl}/${id}`, updateDto, { headers: this.getHeaders() });
  }

  // מחיקת מתנה
  deleteGift(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

}