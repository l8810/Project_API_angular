import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LoginUser } from '../models/user-login.model';
import { CreateUser } from '../models/user-create.model';
import { environment } from '../enviorments/enviorment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private platformId = inject(PLATFORM_ID);
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/auth`;
  private tokenkey = 'auth_token';

  // Subjects קיימים
  private loggedInSubject = new BehaviorSubject<boolean>(this.isLoggedIn());
  loggedIn$ = this.loggedInSubject.asObservable();
  private roleSubject = new BehaviorSubject<string | null>(this.decodeToken()?.role || null);
  role$ = this.roleSubject.asObservable();

  // --- תוספת: Subject לפרטי המשתמש המלאים ---
  private userSubject = new BehaviorSubject<any | null>(this.decodeToken());
  user$ = this.userSubject.asObservable();

  constructor() { }

  login(login: LoginUser): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, login).pipe(
      tap((response: any) => {
        if (response.token && isPlatformBrowser(this.platformId)) {
          localStorage.setItem(this.tokenkey, response.token);
          
          const decoded = this.decodeToken();
          // עדכון כל הזרמים
          this.loggedInSubject.next(true);
          this.roleSubject.next(decoded?.role || null);
          this.userSubject.next(decoded); 
        }
      })
    );
  }

  register(user: CreateUser): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, { email });
  }

  resetPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, { token, newPassword });
  }

  private decodeToken(): any | null {
    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem(this.tokenkey);
      try {
        if (token) {
          const payload = token.split('.')[1];
          // שימוש ב-decodeURIComponent כדי לתמוך בעברית
          return JSON.parse(decodeURIComponent(escape(atob(payload))));
        }
      } catch (error) {
        return null;
      }
    }
    return null;
  }

  isLoggedIn(): boolean {
    if (isPlatformBrowser(this.platformId)) {
      return !!localStorage.getItem(this.tokenkey);
    }
    return false;
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.tokenkey);
    }
  

    this.loggedInSubject.next(false);
    this.roleSubject.next(null);
    this.userSubject.next(null); // זה יגרום ל-userName להפוך ל'אורח'
  }

  getUserName(): string | null {
    const user = this.userSubject.value;
    return user?.unique_name || user?.name || null;
  }

  // מחזיר מזהה המשתמש מתוך ה-JWT אם קיים (nameid, sub, id)
  getUserId(): string | number | null {
    const user = this.userSubject.value;
    if (!user) return null;
    return user?.nameid || user?.sub || user?.id || null;
  }
}