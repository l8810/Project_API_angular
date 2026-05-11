import { Component, OnInit, computed, signal } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { CartItem } from '../../models/cartItem';
import { TableModule } from 'primeng/table';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { FormsModule, ReactiveFormsModule, FormControl, FormGroup, Validators } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';
import { ToastModule } from 'primeng/toast';
import { environment } from '../../enviorments/enviorment';
import { MessageService } from 'primeng/api';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { InputMaskModule } from 'primeng/inputmask'; 
import { InputTextModule } from 'primeng/inputtext';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [
    RouterLink, TableModule, InputNumberModule, ButtonModule, CardModule,
    FormsModule, ToastModule, CommonModule, DialogModule, ReactiveFormsModule,
    InputMaskModule, InputTextModule,
  ],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent implements OnInit {
  cartItems = signal<CartItem[]>([]);
  paymentVisible: boolean = false;
  isProcessing: boolean = false;
  isOrdered: boolean = false;
  buttonLabel: string = 'בצע רכישה';

  paymentForm = new FormGroup({
    cardNumber: new FormControl('', [Validators.required, Validators.pattern('^[0-9-]{19}$')]),
    expiry: new FormControl('', [Validators.required, Validators.pattern('^(0[1-9]|1[0-2])\/?([0-9]{2})$')]),
    cvv: new FormControl('', [Validators.required, Validators.pattern('^[0-9]{3,4}$')]),
    idNumber: new FormControl('', [Validators.required, Validators.pattern('^[0-9]{9}$')])
  });

  constructor(
    private cartService: CartService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.loadCart();
  }

  loadCart() {
    const token = localStorage.getItem('auth_token');
    if (!token) return;

    const decoded: any = jwtDecode(token);
    const userId = Number(decoded.sub);
    
    this.cartService.getCartItems(userId).subscribe(data => {
      this.cartItems.set(data);
    });
  }

  totalItems = computed(() => this.cartItems().reduce((sum, item) => sum + item.count, 0));
  totalPrice = computed(() => this.cartItems().reduce((sum, item) => sum + (item.price * item.count), 0));

  removeItem(id: number) {
    this.cartService.removeItem(id).subscribe({
      next: () => {
        this.cartItems.update(items => items.filter(item => item.id !== id));
        this.messageService.add({ severity: 'success', summary: 'הצלחנו', detail: 'המוצר הוסר מהסל' });
      }
    });
  }

  handleSimpleUpdate(item: CartItem, diff: number) {
    this.cartService.updateCartItemQuantity(item.id, diff).subscribe({
      next: () => {
        this.cartItems.update(items => {
          return items.map(i => i.id === item.id ? { ...i, count: i.count + diff } : i).filter(i => i.count > 0);
        });
      }
    });
  }

  showPayment() {
    this.isOrdered = false;
    this.paymentVisible = true;
  }

  processPayment() {
    if (this.paymentForm.valid) {
      this.isProcessing = true;
      this.buttonLabel = 'מאמת נתונים...';
      
      const token = localStorage.getItem('auth_token');
      if (!token) return;

      const decoded: any = jwtDecode(token);
      const userId = Number(decoded.sub);

      this.cartService.buyCart(userId).subscribe({
        next: () => {
          this.buttonLabel = 'מעבד עסקה...';
          setTimeout(() => {
            this.cartItems.set([]); 
            this.isOrdered = true;  
            this.isProcessing = false;
            this.buttonLabel = 'בצע רכישה';
            this.paymentForm.reset();
          }, 2500);
        },
        error: () => {
          this.isProcessing = false;
          this.buttonLabel = 'בצע רכישה';
          this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'התשלום נכשל' });
        }
      });
    }
  }

  getImageUrl(fileName: string): string {
    if (!fileName) return 'assets/placeholder.png';
    return fileName.startsWith('http') ? fileName : `${environment.apiUrl}/uploads/gifts/${fileName}`;
  }
}