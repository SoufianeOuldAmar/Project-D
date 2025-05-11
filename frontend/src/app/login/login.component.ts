import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http'; // ✅ Add HttpClientModule

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule], // ✅ Import it here
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onLogin() {
    const loginData = this.loginForm.value;
    this.http.post('http://localhost:5244/api/auth/login', loginData)
      .subscribe(
        (response: any) => {
          console.log('Logged in:', response);
        },
        (error) => {
          console.error('Login failed:', error);
        }
      );
  }
}
