import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CartService } from './cart.service';
import { AuthService } from './auth.service';
import { CartItem } from '../models/cartItem';

describe('CartService', () => {
  let service: CartService;
  let http: HttpTestingController;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  const mockItems: CartItem[] = [
    { id: 1, giftId: 10, giftName: 'Watch', giftImage: 'w.jpg', price: 100, count: 2, userId: 5 },
    { id: 2, giftId: 11, giftName: 'Book', giftImage: 'b.jpg', price: 50, count: 1, userId: 5 },
  ];

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getUserId']);
    authServiceSpy.getUserId.and.returnValue(5);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        CartService,
        { provide: AuthService, useValue: authServiceSpy },
      ],
    });

    service = TestBed.inject(CartService);
    http = TestBed.inject(HttpTestingController);
    spyOn(localStorage, 'getItem').and.returnValue('fake-token');
  });

  afterEach(() => {
    http.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  // ── getCartItems ──────────────────────────────────────────────────────────

  it('getCartItems calls GET with userId and returns items', () => {
    service.getCartItems(5).subscribe(items => {
      expect(items.length).toBe(2);
      expect(items[0].giftName).toBe('Watch');
    });

    const req = http.expectOne(r => r.url.includes('/cart/GetCartByUserId'));
    expect(req.request.method).toBe('GET');
    req.flush(mockItems);
  });

  // ── refreshCart ───────────────────────────────────────────────────────────

  it('refreshCart clears items when user is not logged in', () => {
    authServiceSpy.getUserId.and.returnValue(null);
    let emitted: CartItem[] | undefined;
    service.cartItems$.subscribe(items => (emitted = items));

    service.refreshCart();

    expect(emitted).toEqual([]);
  });

  it('refreshCart fetches and emits cart items for logged-in user', fakeAsync(() => {
    let emitted: CartItem[] | undefined;
    service.cartItems$.subscribe(items => (emitted = items));

    service.refreshCart();

    const req = http.expectOne(r => r.url.includes('/cart/GetCartByUserId'));
    req.flush(mockItems);
    tick();

    expect(emitted?.length).toBe(2);
  }));

  // ── removeItem ────────────────────────────────────────────────────────────

  it('removeItem calls DELETE and refreshes cart', fakeAsync(() => {
    service.removeItem(1).subscribe();

    const deleteReq = http.expectOne(r => r.url.includes('/cart/1') && r.method === 'DELETE');
    deleteReq.flush('');
    tick();

    // refreshCart triggers another GET
    const getReq = http.expectOne(r => r.url.includes('/cart/GetCartByUserId'));
    getReq.flush([]);
    tick();
  }));

  // ── updateCartItemQuantity ────────────────────────────────────────────────

  it('updateCartItemQuantity sends PUT with addCount', fakeAsync(() => {
    service.updateCartItemQuantity(1, 3).subscribe();

    const putReq = http.expectOne(r => r.url.includes('/cart/1') && r.method === 'PUT');
    expect(putReq.request.urlWithParams).toContain('addCount=3');
    putReq.flush('');
    tick();

    const getReq = http.expectOne(r => r.url.includes('/cart/GetCartByUserId'));
    getReq.flush([]);
    tick();
  }));

  // ── buyCart ───────────────────────────────────────────────────────────────

  it('buyCart sends POST to Order/buy with userId', fakeAsync(() => {
    service.buyCart(5).subscribe();

    const req = http.expectOne(r => r.url.includes('/Order/buy'));
    expect(req.request.method).toBe('POST');
    expect(req.request.urlWithParams).toContain('userId=5');
    req.flush('');
    tick();

    const getReq = http.expectOne(r => r.url.includes('/cart/GetCartByUserId'));
    getReq.flush([]);
    tick();
  }));
});
