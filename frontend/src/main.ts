import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app/app.component';
import { LoginComponent } from './app/login/login.component';
import { DashboardComponent } from './app/dashboard/dashboard.component';
import { RegisterComponent } from './app/register/register.component';
import { Routes } from '@angular/router';
import { AuthGuard } from './app/auth.guard'; // import guard
import { AgendaComponent } from './app/agenda/agenda.component';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
    { path: 'flight-statistics-agenda', component: AgendaComponent },
    { path: 'touchpoint-statistics-agenda', component: AgendaComponent }
];

bootstrapApplication(AppComponent, {
    providers: [
        provideHttpClient(),
        provideRouter(routes),
    ]
});