import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoginUser } from '../../../models/user-login.model';
import { AuthService } from '../../../services/auth.service';

// PrimeNG Imports
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { PasswordModule } from 'primeng/password';
import { DialogModule } from 'primeng/dialog';
import { RouterLink, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    InputTextModule,
    ButtonModule,
    CardModule,
    PasswordModule,
    DialogModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class Login {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  formSubmitted = false;
  isLoading = false;
  serverError = '';

  // Forgot password dialog
  showForgotDialog = false;
  forgotEmail = '';
  forgotLoading = false;
  forgotSent = false;
  forgotError = '';

  onSubmit() {
    this.formSubmitted = true;
    this.serverError = '';

    if (this.loginForm.valid) {
      this.isLoading = true;
      const loginData: LoginUser = this.loginForm.value;

      this.authService.login(loginData).subscribe({
        next: () => {
          this.isLoading = false;
          this.loginForm.reset();
          this.formSubmitted = false;
          this.router.navigate(['/home']);
        },
        error: (error: any) => {
          this.isLoading = false;
          if (error.status === 401 || error.status === 400) {
            this.serverError = 'האימייל או הסיסמה שגויים. אנא נסה שנית.';
          } else if (error.status === 0) {
            this.serverError = 'לא ניתן להתחבר לשרת. בדוק את החיבור שלך.';
          } else {
            this.serverError = 'אירעה שגיאה. אנא נסה שנית מאוחר יותר.';
          }
        }
      });
    }
  }

  isInvalid(controlName: string) {
    const control = this.loginForm.get(controlName);
    return control?.invalid && (control?.touched || this.formSubmitted);
  }

  getEmailError(): string {
    const control = this.loginForm.get('email');
    if (control?.errors?.['required']) return 'אימייל הוא שדה חובה';
    if (control?.errors?.['email']) return 'כתובת האימייל אינה תקינה';
    return '';
  }

  getPasswordError(): string {
    const control = this.loginForm.get('password');
    if (control?.errors?.['required']) return 'סיסמה היא שדה חובה';
    if (control?.errors?.['minlength']) return 'הסיסמה חייבת להכיל לפחות 8 תווים';
    return '';
  }

  openForgotDialog() {
    const email = this.loginForm.get('email')?.value?.trim();
    if (!email) {
      this.serverError = 'הכנס קודם את כתובת המייל שלך בשדה למעלה';
      return;
    }
    this.forgotEmail = email;
    this.showForgotDialog = true;
    this.forgotSent = false;
    this.forgotError = '';
  }

  onForgotSubmit() {
    this.forgotLoading = true;
    this.forgotError = '';
    this.authService.forgotPassword(this.forgotEmail).subscribe({
      next: () => {
        this.forgotLoading = false;
        this.forgotSent = true;
      },
      error: () => {
        this.forgotLoading = false;
        this.forgotError = 'אירעה שגיאה. אנא נסה שנית מאוחר יותר.';
      }
    });
  }
}