import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let http: HttpTestingController;

  const fakeToken =
    'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.' +
    btoa(JSON.stringify({ nameid: '7', unique_name: 'Alice', role: '1' })) +
    '.sig';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService],
    });
    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    http.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  // ── isLoggedIn ────────────────────────────────────────────────────────────

  it('isLoggedIn returns false when no token in localStorage', () => {
    expect(service.isLoggedIn()).toBeFalse();
  });

  it('isLoggedIn returns true when token exists in localStorage', () => {
    localStorage.setItem('auth_token', fakeToken);
    expect(service.isLoggedIn()).toBeTrue();
  });

  // ── login ─────────────────────────────────────────────────────────────────

  it('login stores token and emits loggedIn=true on success', fakeAsync(() => {
    let loggedIn = false;
    service.loggedIn$.subscribe(v => (loggedIn = v));

    service.login({ email: 'alice@test.com', password: 'pass' }).subscribe();

    const req = http.expectOne(r => r.url.includes('/login'));
    req.flush({ token: fakeToken });
    tick();

    expect(localStorage.getItem('auth_token')).toBe(fakeToken);
    expect(loggedIn).toBeTrue();
  }));

  // ── logout ────────────────────────────────────────────────────────────────

  it('logout clears token and emits loggedIn=false', () => {
    localStorage.setItem('auth_token', fakeToken);
    service.logout();

    expect(localStorage.getItem('auth_token')).toBeNull();
    let loggedIn: boolean | undefined;
    service.loggedIn$.subscribe(v => (loggedIn = v));
    expect(loggedIn).toBeFalse();
  });

  // ── getUserId ─────────────────────────────────────────────────────────────

  it('getUserId returns null when not logged in', () => {
    expect(service.getUserId()).toBeNull();
  });

  it('getUserId returns id from decoded token after login', fakeAsync(() => {
    service.login({ email: 'alice@test.com', password: 'pass' }).subscribe();
    http.expectOne(r => r.url.includes('/login')).flush({ token: fakeToken });
    tick();

    expect(service.getUserId()).toBe('7');
  }));

  // ── getUserName ───────────────────────────────────────────────────────────

  it('getUserName returns null when not logged in', () => {
    expect(service.getUserName()).toBeNull();
  });

  it('getUserName returns name from decoded token after login', fakeAsync(() => {
    service.login({ email: 'alice@test.com', password: 'pass' }).subscribe();
    http.expectOne(r => r.url.includes('/login')).flush({ token: fakeToken });
    tick();

    expect(service.getUserName()).toBe('Alice');
  }));

  // ── forgotPassword ────────────────────────────────────────────────────────

  it('forgotPassword sends POST with email', () => {
    service.forgotPassword('alice@test.com').subscribe();

    const req = http.expectOne(r => r.url.includes('/forgot-password'));
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'alice@test.com' });
    req.flush({});
  });

  // ── resetPassword ─────────────────────────────────────────────────────────

  it('resetPassword sends POST with token and newPassword', () => {
    service.resetPassword('abc123', 'newPass!1').subscribe();

    const req = http.expectOne(r => r.url.includes('/reset-password'));
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ token: 'abc123', newPassword: 'newPass!1' });
    req.flush({});
  });
});
