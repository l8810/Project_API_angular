import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RatingModule } from 'primeng/rating';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { RippleModule } from 'primeng/ripple';
import { OrdersListService } from '../../services/orders-list.service';
import { MessageService } from 'primeng/api';
import { Order } from '../../models/Order';
import { ProductPurches } from '../../models/ProductPurches';
import { environment } from '../../enviorments/enviorment';
import { AuthService } from '../../services/auth.service';
import { jwtDecode } from 'jwt-decode';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [RouterLink, ButtonModule, RatingModule, TableModule, TagModule, ToastModule, RippleModule, FormsModule, CommonModule],
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.css',
})
export class MyOrdersComponent implements OnInit {
  private ordersListService = inject(OrdersListService);
  private messageService = inject(MessageService);
  private authService = inject(AuthService);

  orders = signal<Order[]>([]);
  expandedRows: any = {};

  ngOnInit(): void {
    this.loadOrders();
  }

loadOrders(): void {
  const token = localStorage.getItem('auth_token');

  if (!token) {
    this.messageService.add({
      severity: 'error',
      summary: 'שגיאה',
      detail: 'אנא התחבר מחדש',
      life: 3000
    });
    return;
  }

  try {
    const decoded: any = jwtDecode(token);
    const userId = Number(decoded.sub); // וודא שה-ID נמצא תחת sub ב-Token שלך

    this.ordersListService.getAllOrdersByUserId(userId).subscribe({
      next: (data: Order[]) => {
        console.log('Orders received:', data); // לבדיקה בקונסול
        
        // פשוט מגדירים את הנתונים כפי שהם הגיעו מהשרת
        this.orders.set(data || []);
        
        if (!data || data.length === 0) {
          this.messageService.add({ severity: 'info', summary: 'מידע', detail: 'לא נמצאו הזמנות קודמות', life: 3000 });
        }
      },
      error: (err: any) => {
        console.error('שגיאה בטעינת ההזמנות:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'לא ניתן לטעון את ההזמנות שלך',
          life: 3000
        });
      }
    });
  } catch (e) {
    console.error('Error decoding token:', e);
  }
}

  expandAll() {
    this.expandedRows = this.orders().reduce((acc: any, p: Order) => {
      acc[p.id] = true;
      return acc;
    }, {});
  }

  collapseAll() {
    this.expandedRows = {};
  }

  onRowExpand(event: any) {
    this.messageService.add({ severity: 'info', summary: 'הזמנה הורחבה', detail: `מציג פרטים בהזמנה`, life: 3000 });
  }

  onRowCollapse(event: any) {
    this.messageService.add({ severity: 'success', summary: 'הזמנה צומצמה', detail: `ההזמנה נסגרה`, life: 3000 });
  }

  getTotalPrice(listOrder: ProductPurches[]): number {
    if (!listOrder) return 0;
    return listOrder.reduce((sum, item) => sum + (item.price * item.count), 0);
  }
  getTotalCount(listOrder: ProductPurches[]): number {
    if (!listOrder) return 0;
    return listOrder.reduce((sum, item) => sum + (item.count), 0);
  }

  getImageUrl(fileName: string): string {
    if (!fileName) return 'assets/placeholder.png';
    if (fileName.startsWith('http')) return fileName;
    return `${environment.apiUrl}/uploads/gifts/${fileName}`;
  }

  viewImage(fileName: string): void {
    if (fileName) {
      window.open(this.getImageUrl(fileName), '_blank');
    }
  }
}
