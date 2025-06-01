import { provideRouter, Routes } from '@angular/router';
import { LoginComponent } from '../app/login/login.component';
import { DashboardComponent } from '../app/dashboard/dashboard.component'; // Adjust path if needed
import { RegisterComponent } from './register/register.component';
import { AuthGuard } from './auth.guard'; // import guard
import { AgendaComponent } from './agenda/agenda.component';


export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
    { path: 'flight-statistics-agenda', component: AgendaComponent },
    { path: 'touchpoint-statistics-agenda', component: AgendaComponent },
];

export const appConfig = {
    providers: [provideRouter(routes)],
};
