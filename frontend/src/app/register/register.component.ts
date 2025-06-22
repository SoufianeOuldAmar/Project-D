import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http'; // ✅ Add HttpClientModule
import { Router } from '@angular/router'; // ✅ Import Router
import { NgIf, CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule, NgIf, CommonModule], // ✅ Import HttpClientModule here
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerForm;
  successMessage: string | null = null;
  errorMessage: string | null = null;


  constructor(private fb: FormBuilder, private http: HttpClient, private router: Router) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  onRegister() {
    const registerData = this.registerForm.value;

    // Reset messages before sending request
    this.successMessage = null;
    this.errorMessage = null;

    this.http.post('https://localhost:7150/api/auth/register', registerData).subscribe({
      next: (response: any) => {
        console.log('Registered:', response);
        this.successMessage = 'Registration successful! You can now log in.';
        // Optionally navigate to login page here or after a timeout
        // this.router.navigate(['/login']);
      },
      error: (error) => {
        console.error('Registration failed:', error);
        if (error.status === 400 && typeof error.error === 'string') {
          this.errorMessage = error.error;  // e.g. "Username already exists"
        } else {
          this.errorMessage = 'An unexpected error occurred. Please try again.';
        }
      }
    });
  }

  onLogin() {
    // Redirect to the login page
    this.router.navigate(['/login']);
  }
}
