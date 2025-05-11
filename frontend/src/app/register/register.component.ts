import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http'; // ✅ Add HttpClientModule

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule], // ✅ Import HttpClientModule here
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerForm;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  onRegister() {
    const registerData = this.registerForm.value;
    this.http.post('http://localhost:5244/api/auth/register', registerData)
      .subscribe(
        (response: any) => {
          console.log('Registered:', response);
        },
        (error) => {
          console.error('Registration failed:', error);
        }
      );
  }
}
