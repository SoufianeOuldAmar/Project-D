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
import { AdvancedStatisticsComponent } from './app/advanced-statistics/advanced-statistics.component'; // Adjust path if needed

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
    { path: 'statistics', component: AgendaComponent, canActivate: [AuthGuard] },
    { path: 'advanced-statistics', component: AdvancedStatisticsComponent, canActivate: [AuthGuard] }, // Adjust component as needed

];

bootstrapApplication(AppComponent, {
    providers: [
        provideHttpClient(),
        provideRouter(routes),
    ]
});