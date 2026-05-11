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
    isLoading = false; // לניהול ה-Spinner

    registerForm = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(50)]],
        email: ['', [Validators.required, Validators.email, Validators.maxLength(50)]],
        password: ['', [Validators.required, Validators.minLength(6)]],
    });

    onSubmit() {
        this.formSubmitted = true;
        
        if (this.registerForm.valid) {
            this.isLoading = true; // הפעלת אנימציה
            
            const newUser : CreateUser = {
                name: this.registerForm.value.name ?? '',
                email: this.registerForm.value.email ?? '',
                password: this.registerForm.value.password ?? '',
            }

            this.authService.register(newUser).subscribe({
                next: (response: any) => {
                    this.isLoading = false;
                    this.route.navigate(['/login']);
                },
                error: (error: any) => {
                    this.isLoading = false;
                    // alert('Registration failed');
                }
            });
        }
    }

    isInvalid(controlName: string) {
        const control = this.registerForm.get(controlName);
        return control?.invalid && (control?.touched || this.formSubmitted);
    }
}