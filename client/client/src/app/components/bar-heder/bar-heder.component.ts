import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bar-heder',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './bar-heder.component.html',
  styleUrl: './bar-heder.component.css',
})
export class BarHederComponent implements OnInit {
  private authService = inject(AuthService);
  private cartService = inject(CartService);
  private router = inject(Router);
  userName: string = 'אורח';
  userEmail: string = '';
  isAdmin: boolean = false;
  cartCount: number = 0;

  ngOnInit() {
    // מאזינים לשינויים במשתמש - זה יתעדכן אוטומטית ב-Login וב-Logout
    this.authService.user$.subscribe(user => {
      console.log('BarHederComponent: user changed', user);
      if (user) {
        this.userName = user.name || 'משתמש';
        this.userEmail = user.email || '';
        
        const role = user.role || user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        this.isAdmin = role === '2' || role === 'admin';
        // refresh cart when user changes
        this.cartService.refreshCart();
      } else {
        this.resetUserInfo();
        this.cartService.refreshCart();
      }
    });

    this.cartService.cartItems$.subscribe(items => {
      console.log('BarHederComponent: cartItems$ emitted', items);
      this.cartCount = (items || []).reduce((s, it) => s + (it.count || 0), 0);
    });
  }

  private resetUserInfo() {
    this.userName = 'אורח';
    this.userEmail = '';
    this.isAdmin = false;
  }
  logout() {
    this.authService.logout();
    this.router.navigate(['/login']); 
  }
  isLoggedIn(){
    return this.authService.isLoggedIn();
  }
}