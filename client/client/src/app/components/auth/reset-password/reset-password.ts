import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';

import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';

function passwordMatchValidator(form: AbstractControl) {
    const pw = form.get('password')?.value;
    const confirm = form.get('confirmPassword')?.value;
    return pw === confirm ? null : { mismatch: true };
}

@Component({
    selector: 'app-reset-password',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, InputTextModule, ButtonModule, PasswordModule, RouterLink],
    templateUrl: './reset-password.html',
    styleUrls: ['./reset-password.scss']
})
export class ResetPasswordComponent implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);

    token = '';
    isLoading = false;
    success = false;
    error = '';
    formSubmitted = false;

    resetForm = this.fb.group({
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', [Validators.required]],
    }, { validators: passwordMatchValidator });

    ngOnInit() {
        this.token = this.route.snapshot.queryParamMap.get('token') ?? '';
        if (!this.token) {
            this.error = 'הקישור לאיפוס אינו תקין. אנא בקש קישור חדש.';
        }
    }

    onSubmit() {
        this.formSubmitted = true;
        if (this.resetForm.invalid || !this.token) return;

        this.isLoading = true;
        this.authService.resetPassword(this.token, this.resetForm.value.password!).subscribe({
            next: () => {
                this.isLoading = false;
                this.success = true;
                setTimeout(() => this.router.navigate(['/login']), 3000);
            },
            error: (err: any) => {
                this.isLoading = false;
                this.error = err.error?.message || 'הקישור לאיפוס לא תקין או פג תוקפו.';
            }
        });
    }

    isInvalid(field: string) {
        const control = this.resetForm.get(field);
        return control?.invalid && (control?.touched || this.formSubmitted);
    }

    getPasswordError(): string {
        const c = this.resetForm.get('password');
        if (c?.errors?.['required']) return 'סיסמה היא שדה חובה';
        if (c?.errors?.['minlength']) return 'הסיסמה חייבת להכיל לפחות 8 תווים';
        return '';
    }

    getConfirmError(): string {
        const c = this.resetForm.get('confirmPassword');
        if (c?.errors?.['required']) return 'אנא אמת את הסיסמה';
        if (this.resetForm.errors?.['mismatch']) return 'הסיסמאות אינן תואמות';
        return '';
    }
}
