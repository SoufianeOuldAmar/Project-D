import { Component } from '@angular/core';
import { BackendService } from '../backend.service';
import { CommonModule } from '@angular/common';  // Import CommonModule
import { Router } from '@angular/router'; // ✅ Import Router

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],  // Add CommonModule to imports
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {
  flights: any[] = [];
  touchPoints: any[] = [];
  flightTouchPoints: any[] = [];

  constructor(private backendService: BackendService, private router: Router) { }

  goToDateBasedStatistics() {
    this.router.navigate(['/statistics']);
  }

  goToAdvancedStatistics() {
    this.router.navigate(['/advanced-statistics']);
  }

  clearFlights() {
    this.flights = [];
  }
  clearTouchPoints() {
    this.touchPoints = [];
  }
  clearFlightTouchPoints() {
    this.flightTouchPoints = [];
  }

  logout() {
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('refreshToken');

    // Redirect to the login page
    this.router.navigate(['/login']);
  }

}
