import { Component, OnInit, OnDestroy, signal, inject, PLATFORM_ID } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink,ButtonModule,CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit, OnDestroy {
  // Signal לניהול טקסט הטיימר ב-UI
  countdownText = signal<string>('טוען זמן...');
  isAdmin: boolean = false;
  private authService = inject(AuthService);
  
  private timerInterval: any;
  
  // הזרקת מזהה הפלטפורמה כדי למנוע קריסה בשרת (SSR)
  private platformId = inject(PLATFORM_ID);

  ngOnInit() {
        this.authService.user$.subscribe(user => {
      console.log('BarHederComponent: user changed', user);
      if (user) {
        const role = user.role || user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        this.isAdmin = role === '2' || role === 'admin';
        // refresh cart when user changes
      } else {
        this.isAdmin = false;
      }
    });
    // הרצת הטיימר רק אם הקוד רץ בדפדפן של הלקוח
    if (isPlatformBrowser(this.platformId)) {
      const targetDate = new Date();
      targetDate.setDate(targetDate.getDate() + 12); // הגדרת יעד ל-24 ימים מהיום
      this.startTimer(targetDate);
    } else {
      // תצוגה ראשונית עבור מנועי חיפוש או בזמן טעינה בשרת
      this.countdownText.set('24 ימים');
    }
  }

  startTimer(target: Date) {
    this.timerInterval = setInterval(() => {
      const now = new Date().getTime();
      const distance = target.getTime() - now;

      if (distance < 0) {
        clearInterval(this.timerInterval);
        this.countdownText.set('המכירה הסתיימה');
        return;
      }

      // חישובי זמן
      const days = Math.floor(distance / (1000 * 60 * 60 * 24));
      const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
      const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
      const seconds = Math.floor((distance % (1000 * 60)) / 1000);

      // עדכון ה-Signal
      this.countdownText.set(
        `${this.pad(hours)}:${this.pad(minutes)}:${this.pad(seconds)}`
      );
    }, 1000);
  }

  // עזר לעיצוב מספרים (09 במקום 9)
  private pad(num: number): string {
    return num < 10 ? '0' + num : num.toString();
  }

  ngOnDestroy() {
    // ניקוי הזיכרון ומניעת ריצה ברקע
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }
}