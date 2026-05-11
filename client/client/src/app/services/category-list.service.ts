import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../enviorments/enviorment';
import { Category } from '../models/category';

@Injectable({
  providedIn: 'root',
})
export class CategoryListService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl + '/api/category';

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl, { headers: this.getHeaders() });
  }
}
