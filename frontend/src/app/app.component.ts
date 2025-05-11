import { Component } from '@angular/core';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [LoginComponent, RegisterComponent],
  template: `
    <div>
      <app-login></app-login>
      <app-register></app-register>
    </div>
  `,
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Standalone Angular App';
}
