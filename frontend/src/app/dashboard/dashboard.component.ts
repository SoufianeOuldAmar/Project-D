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

  getFlights(): void {
    this.backendService.getData('flights').subscribe(
      (response) => {
        this.flights = response;
        console.log('Fetched flights:', this.flights);
      },
      (error) => {
        console.error('Error fetching flights:', error);
      }
    );
  }

  getTouchPoints(): void {
    this.backendService.getData('touchpoints').subscribe(
      (response) => {
        this.touchPoints = response.slice(0, 100); // Limit to first 100
        console.log('Fetched touch points:', this.touchPoints);
      },
      (error) => {
        console.error('Error fetching touch points:', error);
      }
    );
  }

  getFlightTouchPoints(): void {
    this.backendService.getData('flights-with-touchpoints').subscribe(
      (response) => {
        this.flightTouchPoints = response.slice(0, 100); // Limit to first 100
        console.log('Fetched flight touch points:', response);
      },
      (error) => {
        console.error('Error fetching flight touch points:', error);
      }
    );
  }

  goToFlightStatistics() { }
  goToTouchPointStatistics() { }


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
