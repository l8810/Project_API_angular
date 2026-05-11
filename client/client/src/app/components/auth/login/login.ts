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
import { RouterLink, Router } from '@angular/router'; // הוספת Router

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
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class Login {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router); // הזרקת הנתב

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    // עדכון ל-8 תווים כפי שביקשת בעיצוב
    password: ['', [Validators.required, Validators.minLength(8)]], 
  });

  formSubmitted = false;

  onSubmit() {
    this.formSubmitted = true;
    
    if (this.loginForm.valid) {
      const loginData: LoginUser = this.loginForm.value;
      
      this.authService.login(loginData).subscribe({
        next: (response: any) => {
          // איפוס הטופס
          this.loginForm.reset();
          this.formSubmitted = false;
          
          // ניתוב לדף המתנות
          this.router.navigate(['/home']); 
        },
        error: (error: any) => {
          console.error('Login failed', error);
          // כאן אפשר להוסיף הצגת הודעת שגיאה למשתמש (למשל עם Toast)
        }
      });
    }
  }

  isInvalid(controlName: string) {
    const control = this.loginForm.get(controlName);
    return control?.invalid && (control?.touched || this.formSubmitted);
  }
}