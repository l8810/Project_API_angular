import { Routes } from '@angular/router';
import  { DonorListComponent } from './components/admin/donor-list/donor-list.component'
import { AdminLayoutComponent } from './components/admin/admin-layout/admin-layout.component';
import { GiftsComponent } from './components/gifts/gifts.component';
import { Register } from './components/auth/register/register';
import {Login} from './components/auth/login/login'
import { GiftListComponent } from './components/admin/gift-list/gift-list.component';
import { OrdersListComponent } from './components/admin/orders-list/orders-list.component';
import { CartComponent } from './components/cart/cart.component';
import { MyOrdersComponent } from './components/my-orders/my-orders.component';
import { DashBoardComponent } from './components/admin/dash-board/dash-board.component';
import { HomeComponent } from './components/home/home.component';

export const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'login', component: Login },
    { path: 'register', component: Register  },
    { path:'gifts', component: GiftsComponent },
    { path:'cart', component: CartComponent },
    { path:'orders', component: MyOrdersComponent },
    {
        path: 'admin', component: AdminLayoutComponent,
        children: [
            { path: 'donors', component: DonorListComponent },
            { path: 'gifts', component: GiftListComponent },
            { path: 'dashboard', component: DashBoardComponent },
            { path: 'orders', component: OrdersListComponent }
        ]
    },
];
