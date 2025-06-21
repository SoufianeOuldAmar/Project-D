import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http'; // ✅ Add HttpClientModule
import { Router } from '@angular/router'; // ✅ Import Router

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule], // ✅ Import it here
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm;

  constructor(private fb: FormBuilder, private http: HttpClient, private router: Router) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onLogin() {
    const loginData = this.loginForm.value;
    this.http.post('https://localhost:7150/api/auth/login', loginData)
      .subscribe(
        (response: any) => {
          console.log('Logged in:', response);

          // Optionally store tokens
          sessionStorage.setItem('accessToken', response.accessToken);
          sessionStorage.setItem('refreshToken', response.refreshToken);

          // Redirect to the desired route (e.g., dashboard)
          this.router.navigate(['/dashboard']);
        },
        (error) => {
          console.error('Login failed:', error);
        }
      );
  }
  toRegister() {
    this.router.navigate(['/register']);
  }

}
