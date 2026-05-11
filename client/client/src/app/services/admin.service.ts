import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { forkJoin, Observable } from 'rxjs';
import { environment } from '../enviorments/enviorment';
import { Gift } from '../models/gift';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/admin`;

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  getStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/stats`, { headers: this.getHeaders() });
  }

raffleAll(gifts: Gift[]): Observable<any[]> {
  const raffleRequests = gifts.map(gift => 
    this.http.post(`${environment.apiUrl}/api/gift/${gift.id}/lottery`, {}, { 
      headers: this.getHeaders(), 
      responseType: 'text' 
    })
  );
  return forkJoin(raffleRequests);
}
    

raffleGift(giftId: number): Observable<string> {
  return this.http.post(`${environment.apiUrl}/api/gift/${giftId}/lottery`, {}, { 
    headers: this.getHeaders(), 
    responseType: 'text'
  });
}
}
