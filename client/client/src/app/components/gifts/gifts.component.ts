import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DataViewModule } from 'primeng/dataview';
import { SelectButtonModule } from 'primeng/selectbutton';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { GiftsService } from '../../services/gifts.service';
import { CartService } from '../../services/cart.service';
import { jwtDecode } from 'jwt-decode';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { environment } from '../../enviorments/enviorment';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-gifts',
  standalone: true,
  imports: [CommonModule, DataViewModule, SelectButtonModule, SelectModule, TagModule, ButtonModule, FormsModule, ToastModule, InputNumberModule],
  providers: [MessageService],
  templateUrl: './gifts.component.html',
  styleUrl: './gifts.component.css'
})
export class GiftsComponent implements OnInit {
  private http = inject(HttpClient);
  private giftsService = inject(GiftsService);
  private cartService = inject(CartService);
  private messageService = inject(MessageService);
  value1: number = 1;

  gifts = signal<any[]>([]);
  layout = signal<'list' | 'grid'>('grid');
  selectedCategory = signal<string | null>(null);
  selectedDonor = signal<string | null>(null);
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  onlyAvailable = signal<boolean>(false);

  options = [
    { label: 'List', value: 'list', icon: 'pi pi-bars' },
    { label: 'Grid', value: 'grid', icon: 'pi pi-table' }
  ];

  categories = computed(() => {
    const cats = [...new Set(this.gifts().map(g => g.categoryName).filter(Boolean))];
    return [{ label: 'כל הקטגוריות', value: null }, ...cats.map(c => ({ label: c, value: c }))];
  });

  donors = computed(() => {
    const dns = [...new Set(this.gifts().map(g => g.donorName).filter(Boolean))];
    return [{ label: 'כל התורמים', value: null }, ...dns.map(d => ({ label: d, value: d }))];
  });

  filteredGifts = computed(() => {
    let result = this.gifts();
    if (this.selectedCategory()) {
      result = result.filter(g => g.categoryName === this.selectedCategory());
    }
    if (this.selectedDonor()) {
      result = result.filter(g => g.donorName === this.selectedDonor());
    }
    if (this.minPrice() !== null) {
      result = result.filter(g => g.price >= this.minPrice()!);
    }
    if (this.maxPrice() !== null) {
      result = result.filter(g => g.price <= this.maxPrice()!);
    }
    if (this.onlyAvailable()) {
      result = result.filter(g => !g.winnerName);
    }
    return result;
  });

  hasActiveFilters = computed(() =>
    this.selectedCategory() !== null ||
    this.selectedDonor() !== null ||
    this.minPrice() !== null ||
    this.maxPrice() !== null ||
    this.onlyAvailable()
  );

  clearFilters() {
    this.selectedCategory.set(null);
    this.selectedDonor.set(null);
    this.minPrice.set(null);
    this.maxPrice.set(null);
    this.onlyAvailable.set(false);
  }

  ngOnInit() {
    this.loadGifts();
  }

  loadGifts() {
    this.giftsService.getAllGifts().subscribe({
      next: (data) => {
        const giftsWithCount = data.map(gift => ({
          ...gift,
          count: 1
        }));
        this.gifts.set(giftsWithCount);
      },
      error: (err) => console.error('Error loading gifts:', err)
    });
  }

  addToCart(giftId: number, count: number) {
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

    const decoded = jwtDecode(token);
    const userId = Number(decoded.sub);

    console.log('GiftsComponent: adding to cart', { giftId, userId, count });

    this.giftsService.addToCart(giftId, userId, count).subscribe({
      next: () => {
        console.log('GiftsComponent: addToCart next');
        this.messageService.add({
          severity: 'success',
          summary: 'הצלחה!',
          detail: 'המוצר נוסף לסל בהצלחה 🎉',
          life: 3000
        });
        // fallback: ensure cart is refreshed for header
        try { this.cartService.refreshCart(); } catch (e) { console.error('GiftsComponent: refreshCart error', e); }
      },
      error: (err) => {
        console.error('GiftsComponent: addToCart error', err);
        if (err && err.status === 200) {
          console.log('GiftsComponent: treating error with status 200 as success');
          this.messageService.add({
            severity: 'success',
            summary: 'הצלחה!',
            detail: 'המוצר נוסף לסל בהצלחה 🎉',
            life: 3000
          });
          try { this.cartService.refreshCart(); } catch(e){ console.error(e); }
        } else {
          this.messageService.add({
            severity: 'error',
            summary: 'שגיאה',
            detail: 'לא הצלחנו להוסיף את המוצר לסל',
            life: 3000
          });
        }
      }
    });
  }

  getSeverity(product: any) {
    switch (product.inventoryStatus) {
      case 'INSTOCK': return 'success';
      case 'LOWSTOCK': return 'warn';
      case 'OUTOFSTOCK': return 'danger';
      default: return 'info';
    }
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