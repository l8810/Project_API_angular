import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../enviorments/enviorment';

@Injectable({
  providedIn: 'root',
})
export class FileUploadService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl + '/api/FileUpload';

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  uploadImage(file: File): Observable<{ url: string; fileName: string }> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{ url: string; fileName: string }>(
      `${this.apiUrl}/upload`, 
      formData, 
      { headers: this.getHeaders() }
    );
  }
}