import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { AdminService } from '../../../services/admin.service';
import { Donor } from '../../../models/donor';
import { DonorListService } from '../../../services/donor-list.service';
import { Gift } from '../../../models/gift';
import { GiftListService } from '../../../services/gift-list.service';
import { Order } from '../../../models/Order';
import { OrdersListService } from '../../../services/orders-list.service';
import { DialogModule } from 'primeng/dialog';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TableModule } from 'primeng/table';
import { RouterModule } from '@angular/router';
import confetti from 'canvas-confetti';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { environment } from '../../../enviorments/enviorment';
import * as pdfMake from 'pdfmake/build/pdfmake';
import * as pdfFonts from 'pdfmake/build/vfs_fonts';

// פונקציית העזר שחייבת להחזיר אובייקט ולא null
const getVfs = () => {
  try {
    const fonts = pdfFonts as any;
    const vfs = fonts?.pdfMake?.vfs || fonts?.vfs || (window as any).pdfMake?.vfs;
    return vfs || {}; // תמיד מחזיר אובייקט
  } catch (e) {
    return {};
  }
};

@Component({
  selector: 'app-dash-board',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule, DividerModule, ToastModule, DialogModule, ProgressSpinnerModule, ConfirmDialogModule, TableModule, RouterModule],
  templateUrl: './dash-board.component.html',
  styleUrl: './dash-board.component.css',
  providers: [MessageService, ConfirmationService]
})
export class DashBoardComponent implements OnInit {
  private messageService = inject(MessageService);
  private donorListService = inject(DonorListService);
  private giftListService = inject(GiftListService);
  private ordersListService = inject(OrdersListService);
  private adminService = inject(AdminService);
  private confirmationService = inject(ConfirmationService);


  // סיגנלים לנתוני הבסיס
  loading = signal(false);
  donors = signal<Donor[]>([]);
  gifts = signal<Gift[]>([]);
  orders = signal<Order[]>([]);

  isRaffling = signal(false);
  raffleDialogVisible = signal(false);
  currentRaffleGift = signal<string | null>(null);
  winnerName = signal<string | null>(null);
  winnersDialogVisible = signal(false);


  filteredGifts = computed(() => {
    return this.gifts().filter(g => !g.winnerName);
  });
  // חישוב אוטומטי של סכום כולל (מתעדכן בכל פעם ש-orders משתנה)
  totalSum = computed(() => {
    return this.orders().reduce((total, order) => {
      const orderSum = order.listOrder?.reduce((s, p) => s + (p.price * p.count), 0) || 0;
      return total + orderSum;
    }, 0);
  });

  // חישוב אוטומטי של כמות כרטיסים כוללת
  totalTickets = computed(() => {
    return this.orders().reduce((total, order) => {
      const orderTickets = order.listOrder?.reduce((s, p) => s + p.count, 0) || 0;
      return total + orderTickets;
    }, 0);
  });

  // אובייקט סטטיסטיקה מרוכז שנגזר מהסיגנלים האחרים
  stats = computed(() => ({
    tickets: this.totalTickets(),
    revenue: this.totalSum(),
    donorsCount: this.donors().length,
    giftsCount: this.gifts().length
  }));

  ngOnInit(): void {
    this.loadAllData();
  }

  loadAllData(): void {
    this.loading.set(true);
    this.loadDonors();
    this.loadGifts();
    this.loadOrders();
  }

  loadOrders(): void {
    this.ordersListService.getAllOrders().subscribe({
      next: (data: Order[]) => {
        this.orders.set(data || []);
        console.log('DashBoard: loaded orders', (data || []).length);
        console.log('DashBoard: totalTickets now', this.totalTickets());
        console.log('DashBoard: totalSum now', this.totalSum());
        this.checkLoadingComplete();
      },
      error: (err) => this.handleError('שגיאה בטעינת הזמנות', err)
    });
  }

  loadGifts(): void {
    this.giftListService.getAllGifts().subscribe({
      next: (data: Gift[]) => {
        this.gifts.set(data || []);
        console.log('DashBoard: loaded gifts', (data || []).length);
        this.checkLoadingComplete();
      },
      error: (err) => this.handleError('שגיאה בטעינת מתנות', err)
    });
  }

  loadDonors(): void {
    this.donorListService.getAllDonors().subscribe({
      next: (data: Donor[]) => {
        this.donors.set(data || []);
        console.log('DashBoard: loaded donors', (data || []).length);
        this.checkLoadingComplete();
      },
      error: (err) => this.handleError('שגיאה בטעינת תורמים', err)
    });
  }

  private checkLoadingComplete(): void {
    // ניתן להוסיף לוגיקה שבודקת אם כל הנתונים הגיעו לפני כיבוי ה-loading
    this.loading.set(false);
  }

  private handleError(summary: string, err: any): void {
    console.error(summary, err);
    this.loading.set(false);
    this.messageService.add({
      severity: 'error',
      summary: summary,
      detail: 'בדוק את חיבור השרת',
      life: 3000
    });
  }

  raffleSingle(giftId: number, giftName: string) {
    const hasParticipants = this.orders().some(order =>
      order.listOrder?.some(item => item.giftId === giftId)
    );

    if (!hasParticipants) {
      this.messageService.add({
        severity: 'warn',
        summary: 'לא ניתן להגריל',
        detail: `לא נרכשו כרטיסים עבור "${giftName}"`,
        life: 5000
      });
      return;
    }

    this.confirmationService.confirm({
      message: `האם אתה בטוח שברצונך לבצע הגרלה על "${giftName}"?`,
      header: 'אישור הגרלה',
      icon: 'pi pi-question-circle',
      acceptLabel: 'כן, הגרל עכשיו',
      rejectLabel: 'ביטול',
      acceptButtonStyleClass: 'p-button-success',
      rejectButtonStyleClass: 'p-button-text p-button-secondary',
      accept: () => {
        this.executeSingleRaffle(giftId, giftName);
      }
    });
  }
  private executeSingleRaffle(giftId: number, giftName: string) {
    this.currentRaffleGift.set(giftName);
    this.winnerName.set(null);
    this.raffleDialogVisible.set(true);
    this.isRaffling.set(true);

    this.adminService.raffleGift(giftId).subscribe({
      next: (response: any) => {
        let finalName = 'זוכה לא ידוע';
        try {
          let data = typeof response === 'string' ? JSON.parse(response) : response;
          finalName = data.name || data.Name || data.winnerName || data.userName || 'זוכה';
        } catch (e) {
          finalName = response;
        }

        setTimeout(() => {
          this.winnerName.set(finalName);
          this.isRaffling.set(false);

          // הוספת אפקט הקונפטי ברגע החשיפה!
          this.launchConfetti();

          this.messageService.add({
            severity: 'success',
            summary: `הגרלה הושלמה`,
            detail: `הזוכה ב${giftName}: ${finalName}`,
            life: 5000
          });
          this.loadAllData();
        }, 2000);
      },
      error: (err) => {
        this.raffleDialogVisible.set(false);
        this.handleError('ההגרלה נכשלה', err);
      }
    });
  }

  // פונקציית העזר ליצירת החגיגה
  private launchConfetti() {
    const duration = 3 * 1000;
    const end = Date.now() + duration;

    const frame = () => {
      confetti({
        particleCount: 3,
        angle: 60,
        spread: 55,
        origin: { x: 0 }, // צד שמאל
        colors: ['#22C55E', '#3B82F6', '#F59E0B'] // צבעי המותג שלך (ירוק, כחול, כתום)
      });
      confetti({
        particleCount: 3,
        angle: 120,
        spread: 55,
        origin: { x: 1 }, // צד ימין
        colors: ['#22C55E', '#3B82F6', '#F59E0B']
      });

      if (Date.now() < end) {
        requestAnimationFrame(frame);
      }
    };
    frame();
  }
  raffleAll(): void {
    // 1. סינון מתנות שטרם הוגרלו
    const unraffledGifts = this.gifts().filter(g => !g.winnerName);

    // 2. סינון נוסף: רק אלו שיש להן רוכשים בפועל
    const giftsWithParticipants = unraffledGifts.filter(gift =>
      this.orders().some(order => order.listOrder?.some(item => item.giftId === gift.id))
    );

    if (unraffledGifts.length === 0) {
      this.messageService.add({ severity: 'info', summary: 'אין מתנות', detail: 'הכל כבר הוגרל' });
      return;
    }

    if (giftsWithParticipants.length === 0) {
      this.messageService.add({
        severity: 'error',
        summary: 'שגיאת הגרלה',
        detail: 'לא ניתן לבצע הגרלה המונית כי לא נמצאו רוכשי כרטיסים לאף מתנה שנותרה.'
      });
      return;
    }

    // אם יש מתנות ללא משתתפים, נעדכן את המשתמש שנדלג עליהן
    if (giftsWithParticipants.length < unraffledGifts.length) {
      const diff = unraffledGifts.length - giftsWithParticipants.length;
      this.messageService.add({
        severity: 'info',
        summary: 'שימו לב',
        detail: `המערכת תגריל ${giftsWithParticipants.length} מתנות. על ${diff} מתנות נדלג מחוסר משתתפים.`,
        life: 6000
      });
    }

    this.confirmationService.confirm({
      message: `האם אתה בטוח שברצונך להגריל את ${giftsWithParticipants.length} המתנות שיש להן משתתפים?`,
      header: 'אישור הגרלה המונית',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'כן, הגרל הכל',
      rejectLabel: 'ביטול',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.executeRaffleAll(giftsWithParticipants);
      }
    });
  }

  // הוצאנו את הלוגיקה לפונקציה נפרדת כדי לשמור על קוד נקי
  private executeRaffleAll(remainingGifts: Gift[]): void {
    this.currentRaffleGift.set('כל המתנות שנותרו');
    this.winnerName.set(null);
    this.raffleDialogVisible.set(true);
    this.isRaffling.set(true);

    this.adminService.raffleAll(remainingGifts).subscribe({
      next: (results: string[]) => {
        setTimeout(() => {
          this.isRaffling.set(false);
          this.winnerName.set(`הושלמו ${results.length} הגרלות בהצלחה!`);

          // הפעלת חגיגה עוצמתית במיוחד להגרלה המונית
          this.launchMassiveConfetti();

          this.loadAllData();
        }, 2000);
      },
      error: (err) => {
        this.raffleDialogVisible.set(false);
        this.handleError('ההגרלה ההמונית נכשלה', err);
      }
    });
  }

  // פונקציה חדשה לחגיגה גדולה - "מטר של קונפטי" מהמרכז
  private launchMassiveConfetti() {
    const duration = 5 * 1000; // חגיגה ארוכה יותר (5 שניות)
    const animationEnd = Date.now() + duration;
    const defaults = { startVelocity: 30, spread: 360, ticks: 60, zIndex: 0 };

    const randomInRange = (min: number, max: number) => Math.random() * (max - min) + min;

    const interval: any = setInterval(() => {
      const timeLeft = animationEnd - Date.now();

      if (timeLeft <= 0) {
        return clearInterval(interval);
      }

      const particleCount = 50 * (timeLeft / duration);

      // ירייה משני מוקדים שונים בו זמנית
      confetti({ ...defaults, particleCount, origin: { x: randomInRange(0.1, 0.3), y: Math.random() - 0.2 } });
      confetti({ ...defaults, particleCount, origin: { x: randomInRange(0.7, 0.9), y: Math.random() - 0.2 } });
    }, 250);
  }

  // פונקציית עזר לבדיקה האם למתנה יש משתתפים
  hasParticipants(giftId: number): boolean {
    return this.orders().some(order =>
      order.listOrder?.some(item => item.giftId === giftId)
    );
  }

  //בשביל דוח הזוכים
  winnersList = computed(() => {
    return this.gifts().filter(g => g.winnerName);
  });
  showWinnersReport() {
    if (this.winnersList().length === 0) {
      this.messageService.add({
        severity: 'info',
        summary: 'אין זוכים',
        detail: 'טרם בוצעו הגרלות במערכת'
      });
      return;
    }
    this.winnersDialogVisible.set(true);
  }
  downloadWinnersPDF() {
    console.log('--- התחלת תהליך יצירת PDF ---');

    if (typeof window === 'undefined') return;

    const winners = this.winnersList();
    const vfs = getVfs(); // וודא שפונקציה זו מחזירה {} במקרה הכי גרוע

    if (!winners || winners.length === 0) {
      this.messageService.add({ severity: 'warn', summary: 'אין נתונים', detail: 'אין זוכים להצגה בדוח' });
      return;
    }

    // 1. הגדרת הגופנים (Fonts)
    const fonts = {
      HebrewFont: {
        normal: 'https://raw.githubusercontent.com/google/fonts/main/ofl/heebo/Heebo%5Bwght%5D.ttf',
        bold: 'https://raw.githubusercontent.com/google/fonts/main/ofl/heebo/Heebo%5Bwght%5D.ttf',
        italics: 'https://raw.githubusercontent.com/google/fonts/main/ofl/heebo/Heebo%5Bwght%5D.ttf',
        bolditalics: 'https://raw.githubusercontent.com/google/fonts/main/ofl/heebo/Heebo%5Bwght%5D.ttf'
      }
    };

    // 2. הגדרת מבנה המסמך (Document Definition)
    const documentDefinition: any = {
      content: [
        { text: this.fixTextDirection('דוח זוכים - מכירה סינית'), style: 'header' }, {
          table: {
            headerRows: 1,
            widths: ['*', '*', '*'],
            body: [
              [
                { text: 'סטטוס', style: 'tableHeader' },
                { text: 'שם הזוכה', style: 'tableHeader' },
                { text: 'מתנה', style: 'tableHeader' }
              ],
              ...winners.map(gift => [
                { text: 'הוגרל', alignment: 'right' },
                { text: this.fixTextDirection(gift.winnerName || 'ללא שם'), alignment: 'right' },
                { text: this.fixTextDirection(gift.name || 'ללא שם'), alignment: 'right' }
              ])
            ]
          },
          layout: 'lightHorizontalLines'
        }
      ],
      rtl: true,
      defaultStyle: {
        font: 'HebrewFont',
        alignment: 'right'
      },
      styles: {
        header: { fontSize: 22, bold: true, alignment: 'center', margin: [0, 0, 0, 20] },
        tableHeader: { bold: true, fontSize: 13, color: 'white', fillColor: '#10b981', alignment: 'right' }
      }
    };
    try {
      console.log('ניסיון יצירה סופי...');

      const vfs = getVfs();
      const pMake = (pdfMake as any).default || pdfMake;

      // הגדרה גלובלית
      pMake.vfs = vfs;
      pMake.fonts = fonts;

      // השינוי כאן: העברנו {} במקום null כפרמטר השני
      // המבנה הוא: createPdf(docDefinition, tableLayouts, fonts, vfs)
      const pdfDocGenerator = pMake.createPdf(documentDefinition, {}, fonts, vfs);

      pdfDocGenerator.download('winners_report.pdf');

      console.log('בוצע בהצלחה!');

    } catch (error) {
      console.error('שגיאה בפרמטרים של createPdf:', error);
    }
  }

fixTextDirection(text: string): string {
  if (!text) return '';

  // ניקוי רווחים כפולים ומיותרים מהמקור
  const cleanText = text.trim().replace(/\s+/g, ' ');
  
  const hasHebrew = /[\u0590-\u05FF]/.test(cleanText);

  if (hasHebrew) {
    // הפיכת סדר המילים
    // אנחנו משתמשים ברווח כפול '  ' כדי להבטיח שהמנוע לא יצמיד אותן
    return cleanText.split(' ').reverse().join('  ');
  }

  return cleanText;
}

  getImageUrl(fileName: string): string {
    if (!fileName) return 'assets/placeholder.png';

    if (fileName.startsWith('http')) return fileName;

    return `${environment.apiUrl}/uploads/gifts/${fileName}`;
  }
}