import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Donor } from '../models/donor';
import { environment } from '../enviorments/enviorment';

@Injectable({
  providedIn: 'root'
})
export class DonorListService {
  private http = inject(HttpClient);
  // private apiUrl = 'http://localhost:5186/api/Donor';
  private apiUrl=environment.apiUrl+'/api/Donor';

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // קבלת כל התורמים
  getAllDonors(): Observable<Donor[]> {
    return this.http.get<Donor[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  // קבלת תורם לפי ID
  getDonorById(id: number): Observable<Donor> {
    return this.http.get<Donor>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  // יצירת תורם חדש
  createDonor(donor: Donor): Observable<Donor> {
    return this.http.post<Donor>(this.apiUrl, donor, { headers: this.getHeaders() });
  }

  // עדכון תורם קיים
  updateDonor(id: number, donor: Donor): Observable<Donor> {
    return this.http.put<Donor>(`${this.apiUrl}/${id}`, donor, { headers: this.getHeaders() });
  }

  // מחיקת תורם
  deleteDonor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}