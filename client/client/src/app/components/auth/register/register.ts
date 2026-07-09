import { Component, inject } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { Router, RouterLink } from "@angular/router";
import { CreateUser } from "../../../models/user-create.model";
import { AuthService } from "../../../services/auth.service";

// PrimeNG
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [
        ReactiveFormsModule, 
        CommonModule, 
        RouterLink, 
        InputTextModule, 
        ButtonModule, 
        PasswordModule
    ],
    templateUrl: './register.html',
    styleUrls: ['./register.scss']
})
export class Register {
    authService = inject(AuthService);
    fb = inject(FormBuilder);
    route = inject(Router);

    formSubmitted = false;
    isLoading = false;
    serverError = '';

    registerForm = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(50)]],
        email: ['', [Validators.required, Validators.email, Validators.maxLength(50)]],
        password: ['', [Validators.required, Validators.minLength(8)]],
    });

    onSubmit() {
        this.formSubmitted = true;
        this.serverError = '';

        if (this.registerForm.valid) {
            this.isLoading = true;

            const newUser: CreateUser = {
                name: this.registerForm.value.name ?? '',
                email: this.registerForm.value.email ?? '',
                password: this.registerForm.value.password ?? '',
            };

            this.authService.register(newUser).subscribe({
                next: () => {
                    this.isLoading = false;
                    this.route.navigate(['/login']);
                },
                error: (error: any) => {
                    this.isLoading = false;
                    if (error.status === 409 || error.error?.message?.includes('already registered')) {
                        this.serverError = 'כתובת האימייל כבר רשומה במערכת. נסה להתחבר במקום.';
                    } else if (error.status === 400) {
                        this.serverError = 'הנתונים שהוזנו אינם תקינים. אנא בדוק את הפרטים.';
                    } else if (error.status === 0) {
                        this.serverError = 'לא ניתן להתחבר לשרת. בדוק את החיבור שלך.';
                    } else {
                        this.serverError = 'אירעה שגיאה בהרשמה. אנא נסה שנית מאוחר יותר.';
                    }
                }
            });
        }
    }

    isInvalid(controlName: string) {
        const control = this.registerForm.get(controlName);
        return control?.invalid && (control?.touched || this.formSubmitted);
    }

    getNameError(): string {
        const control = this.registerForm.get('name');
        if (control?.errors?.['required']) return 'שם מלא הוא שדה חובה';
        if (control?.errors?.['maxlength']) return 'השם לא יכול להכיל יותר מ-50 תווים';
        return '';
    }

    getEmailError(): string {
        const control = this.registerForm.get('email');
        if (control?.errors?.['required']) return 'אימייל הוא שדה חובה';
        if (control?.errors?.['email']) return 'כתובת האימייל אינה תקינה';
        if (control?.errors?.['maxlength']) return 'האימייל ארוך מדי';
        return '';
    }

    getPasswordError(): string {
        const control = this.registerForm.get('password');
        if (control?.errors?.['required']) return 'סיסמה היא שדה חובה';
        if (control?.errors?.['minlength']) return 'הסיסמה חייבת להכיל לפחות 8 תווים';
        return '';
    }
}