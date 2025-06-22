import { Component } from '@angular/core';
import { BackendService } from '../backend.service';
import { Router } from '@angular/router';
import { ChartType, ChartOptions } from 'chart.js';
import { NgIf, NgFor } from '@angular/common';
import { CommonModule } from '@angular/common';  // <-- Import this
import { FormsModule } from '@angular/forms';  // <-- Import this for ngModel
import { NgChartsModule } from 'ng2-charts';


export interface TrafficTypeStats {
  trafficType: string;
  count: number;
  percentage: number;
}

export interface TrafficTypeResponse {
  mostCommonType: TrafficTypeStats;
  allTypes: TrafficTypeStats[];
}


export interface FlightsPerAirport {
  airport: string;
  flightCount: number;
}

export interface TopCountry {
  country: string;
  flightCount: number;
  mostPopularAirport: string;
}

export interface BusyHourStats {
  day: string;
  busiestPeriod: string;
  touchpointCount: number;
}


@Component({
  selector: 'app-advanced-statistics',
  imports: [NgFor, NgIf, CommonModule, FormsModule, NgChartsModule], // <-- Add CommonModule and FormsModule here
  templateUrl: './advanced-statistics.component.html',
  styleUrl: './advanced-statistics.component.css'
})
export class AdvancedStatisticsComponent {

  constructor(private backendService: BackendService) { }

  trafficTypeStats: TrafficTypeResponse | null = null;
  flightsPerAirport: FlightsPerAirport[] = [];
  topCountries: TopCountry[] = [];
  busyHours: BusyHourStats[] = [];
  showGraphs: boolean = false; // Toggle for showing graphs
  barChartType: ChartType = 'bar';

  getBarChartOptions(title: string): ChartOptions {
    return {
      responsive: true,
      plugins: {
        title: {
          display: true,
          text: title,
          font: {
            size: 18
          }
        },
        legend: {
          display: true,
          position: 'bottom',
        }
      }
    };
  }

  getTrafficType(): void {
    this.backendService.getData('touchpoints/statistics/traffic-type').subscribe(
      (response: TrafficTypeResponse) => {
        this.trafficTypeStats = response;
        console.log('Traffic type stats:', response);
      },
      (error) => {
        console.error('Error fetching traffic type stats:', error);
      }
    );
  }


  getFlightsPerAirport(): void {
    this.backendService.getData('touchpoints/statistics/flights-per-airport').subscribe(
      (response: FlightsPerAirport[]) => {
        this.flightsPerAirport = response;
      },
      (error) => {
        console.error('Error fetching flights per airport:', error);
      }
    );
  }

  getTopCountries(top: number = 10): void {

    if (top < 1 || top > 15) {
      console.error('Top value must be between 1 and 15');
      return;
    }

    this.backendService.getData(`touchpoints/statistics/top-countries?top=${top}`).subscribe(
      (response: TopCountry[]) => {
        this.topCountries = response;
        console.log('Top countries:', response);
      },
      (error) => {
        console.error('Error fetching top countries:', error);
      }
    );
  }

  getBusyHours(timeWindowMinutes: number = 60): void {
    this.backendService.getData(`touchpoints/statistics/busy-hours?timeWindowMinutes=${timeWindowMinutes}`).subscribe(
      (response: BusyHourStats[]) => {
        this.busyHours = response;
        console.log('Busy hours per day:', response);
      },
      (error) => {
        console.error('Error fetching busy hours:', error);
      }
    );
  }

  get trafficTypeChartData(): any {
    if (!this.trafficTypeStats) {
      return { labels: [], datasets: [] };
    }

    const labels = this.trafficTypeStats.allTypes.map(type => type.trafficType);
    const data = this.trafficTypeStats.allTypes.map(type => type.count);

    return {
      labels,
      datasets: [{
        label: 'Verkeerstype Aantal',  // <-- add this
        data,
        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF'],
        hoverBackgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF']
      }]
    };
  }

  get flightsPerAirportChartData(): any {
    if (!this.flightsPerAirport || this.flightsPerAirport.length === 0) {
      return { labels: [], datasets: [] };  // also fix this: labels should be array not string
    }

    const labels = this.flightsPerAirport.map(airport => airport.airport);
    const data = this.flightsPerAirport.map(airport => airport.flightCount);

    return {
      labels,
      datasets: [{
        label: 'Aantal Vluchten',  // <-- add label here
        data,
        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF'],
        hoverBackgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF']
      }]
    };
  }

  // Same for other chart getters:

  get topCountriesChartData(): any {
    if (!this.topCountries || this.topCountries.length === 0) {
      return { labels: [], datasets: [] };
    }

    const labels = this.topCountries.map(country => country.country);
    const data = this.topCountries.map(country => country.flightCount);

    return {
      labels,
      datasets: [{
        label: 'Aantal Vluchten per Land',  // <-- add label
        data,
        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF'],
        hoverBackgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF']
      }]
    };
  }

  get busyHoursChartData(): any {
    if (!this.busyHours || this.busyHours.length === 0) {
      return { labels: [], datasets: [] };
    }

    const labels = this.busyHours.map(hour => hour.day + ' ' + hour.busiestPeriod);
    const data = this.busyHours.map(hour => hour.touchpointCount);

    return {
      labels,
      datasets: [{
        label: 'Aantal Touchpoints',  // <-- add label
        data,
        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF'],
        hoverBackgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF']
      }]
    };
  }


  clearTrafficTypeStats(): void {
    this.trafficTypeStats = null;
  }

  clearFlightsPerAirport(): void {
    this.flightsPerAirport = [];
  }

  clearTopCountries(): void {
    this.topCountries = [];
  }

  clearBusyHours(): void {
    this.busyHours = [];
  }
}
