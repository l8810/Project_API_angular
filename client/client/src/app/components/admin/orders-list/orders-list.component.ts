import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RatingModule } from 'primeng/rating';
import { TableModule, TableRowCollapseEvent, TableRowExpandEvent } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { RippleModule } from 'primeng/ripple';
import { OrdersListService } from '../../../services/orders-list.service';
import { MessageService } from 'primeng/api';
import { Order } from '../../../models/Order';
import { ProductPurches } from '../../../models/ProductPurches';
import { environment } from '../../../enviorments/enviorment';

@Component({
  selector: 'app-orders-list',
  standalone: true,
  imports: [
    ButtonModule, RatingModule, TableModule, TagModule, 
    ToastModule, RippleModule, FormsModule, CommonModule
  ],
  templateUrl: './orders-list.component.html',
  styleUrl: './orders-list.component.css',
})
export class OrdersListComponent implements OnInit {
  private ordersListService = inject(OrdersListService);
  private messageService = inject(MessageService);
  
  orders = signal<Order[]>([]);
  expandedRows: any = {};

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.ordersListService.getAllOrders().subscribe({
      next: (data: Order[]) => this.orders.set(data),
      error: (err: any) => {
        console.error('שגיאה בטעינת הזמנות:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'לא ניתן לטעון את רשימת ההזמנות',
          life: 3000
        });
      }
    });
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

  getSeverity(status: string): any {
      switch (status) {
          case 'INSTOCK': return 'success';
          case 'LOWSTOCK': return 'warn';
          case 'OUTOFSTOCK': return 'danger';
          default: return 'info';
      }
  }

  getStatusSeverity(status: string): any {
      switch (status) {
          case 'PENDING': return 'warn';
          case 'DELIVERED': return 'success';
          case 'CANCELLED': return 'danger';
          default: return 'info';
      }
  }

  onRowExpand(event: any) {
      this.messageService.add({ 
        severity: 'info', 
        summary: 'הזמנה הורחבה', 
        detail: `מציג פרטים עבור: ${event.data.userName}`, 
        life: 3000 
      });
  }

  onRowCollapse(event: any) {
      this.messageService.add({ 
        severity: 'success', 
        summary: 'הזמנה צומצמה', 
        detail: `נסגרו פרטי ההזמנה של: ${event.data.userName}`, 
        life: 3000 
      });
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