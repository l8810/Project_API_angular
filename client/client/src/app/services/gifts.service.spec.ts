import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { GiftsService } from './gifts.service';
import { CartService } from './cart.service';
import { Gift } from '../models/gift';

describe('GiftsService', () => {
  let service: GiftsService;
  let http: HttpTestingController;
  let cartServiceSpy: jasmine.SpyObj<CartService>;

  const mockGifts: Gift[] = [
    { id: 1, name: 'Watch', description: 'Elegant watch', price: 200, donorId: 1, categoryId: 1, picture: 'w.jpg' },
    { id: 2, name: 'Book Set', description: 'Classic novels', price: 80, donorId: 2, categoryId: 2, picture: 'b.jpg' },
  ];

  beforeEach(() => {
    cartServiceSpy = jasmine.createSpyObj('CartService', ['refreshCart']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        GiftsService,
        { provide: CartService, useValue: cartServiceSpy },
      ],
    });

    service = TestBed.inject(GiftsService);
    http = TestBed.inject(HttpTestingController);
    spyOn(localStorage, 'getItem').and.returnValue('fake-token');
  });

  afterEach(() => {
    http.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  // ── getAllGifts ────────────────────────────────────────────────────────────

  it('getAllGifts returns list of gifts from API', fakeAsync(() => {
    let result: Gift[] = [];
    service.getAllGifts().subscribe(gifts => (result = gifts));

    const req = http.expectOne(r => r.url.includes('/Gift'));
    expect(req.request.method).toBe('GET');
    req.flush(mockGifts);
    tick();

    expect(result.length).toBe(2);
    expect(result[0].name).toBe('Watch');
    expect(result[1].name).toBe('Book Set');
  }));

  it('getAllGifts returns empty array when API returns empty', fakeAsync(() => {
    let result: Gift[] = [{ id: 99 } as any];
    service.getAllGifts().subscribe(gifts => (result = gifts));

    http.expectOne(r => r.url.includes('/Gift')).flush([]);
    tick();

    expect(result).toEqual([]);
  }));

  // ── addToCart ─────────────────────────────────────────────────────────────

  it('addToCart sends POST with correct body', fakeAsync(() => {
    service.addToCart(10, 5, 2).subscribe();

    const req = http.expectOne(r => r.url.includes('/Cart'));
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ giftId: 10, userId: 5, count: 2 });
    req.flush('Item added');
    tick();

    expect(cartServiceSpy.refreshCart).toHaveBeenCalledOnceWith();
  }));

  it('addToCart refreshes cart on success', fakeAsync(() => {
    service.addToCart(1, 1, 1).subscribe();

    http.expectOne(r => r.url.includes('/Cart')).flush('ok');
    tick();

    expect(cartServiceSpy.refreshCart).toHaveBeenCalled();
  }));
});
