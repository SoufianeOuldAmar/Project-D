import { provideRouter, Routes } from '@angular/router';
import { LoginComponent } from '../app/login/login.component';
import { DashboardComponent } from '../app/dashboard/dashboard.component'; // Adjust path if needed
import { RegisterComponent } from './register/register.component';
import { AuthGuard } from './auth.guard'; // import guard
import { AgendaComponent } from './agenda/agenda.component';
import { AdvancedStatisticsComponent } from './advanced-statistics/advanced-statistics.component'; // Adjust path if needed


export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
    { path: 'statistics', component: AgendaComponent, canActivate: [AuthGuard] },
    { path: 'advanced-statistics', component: AdvancedStatisticsComponent, canActivate: [AuthGuard] }, // Adjust component as needed
];

export const appConfig = {
    providers: [provideRouter(routes)],
};
