import { Component } from '@angular/core';
import { NgFor, NgIf, CommonModule } from '@angular/common';
import { BackendService } from '../backend.service';
import { Router } from '@angular/router'; // ✅ Import Router
import { NgChartsModule } from 'ng2-charts';
import { ChartData, ChartType } from 'chart.js';
import { FormsModule } from '@angular/forms';
import { ChartOptions } from 'chart.js';

interface AirportStatistic {
  airport: string;
  flightCount: number;
}

interface PaxStatistics {
  male: number;
  female: number;
  infant: number;
  child: number;
  total: number;
  terminal: number;
  transit: number;
}

interface BaggageStatistics {
  totalWeight: number;
  terminalWeight: number;
  transitWeight: number;
  totalBags: number;
  terminalBags: number;
  transitBags: number;
}

export interface FlightStatistics {
  startDate: string;
  endDate: string;
  delayedFlights: number;
  arrivingFlights: number;
  divertedFlights: number;
  nachtvluchtFlights: number;
  mostPopularAirports: AirportStatistic[];
  totalSeats: number;
  euFlights: number;
  nonEuFlights: number;
  paxStatistics: PaxStatistics;
  baggageStatistics: BaggageStatistics;
  totalFlights: number;
}

export interface TouchPointStatisticsYearly {
  year: number;
  flightCount: number;
  totalPassengers: number;
  averageDelay: number;
  topAirports: {
    airport: string;
    count: number;
  }[];
}


export interface TouchPointStatisticsMonthly {
  year: number;
  month: string;
  flightCount: number;
  totalPassengers: number;
  busiestDay: number;
  topAircraftTypes: string[];
  message: string | null;
}


@Component({
  selector: 'app-agenda',
  templateUrl: './agenda.component.html',
  imports: [NgFor, NgIf, CommonModule, NgChartsModule, FormsModule],
  styleUrls: ['./agenda.component.css']
})
export class AgendaComponent {
  flightStatisticsByYear: FlightStatistics | null = null;
  flightStatisticsByYearMonth: FlightStatistics | null = null;
  flightStatisticsByYearMonthDay: FlightStatistics | null = null;
  touchPointStatisticsByYear: TouchPointStatisticsYearly | null = null;
  touchPointStatisticsByMonth: TouchPointStatisticsMonthly | null = null;

  selectedYear: number | null = null;
  selectedMonth: number | null = null;
  selectedDay: number | null = null;
  days: number[] = [];
  barChartType: ChartType = 'bar';
  showGraphs: boolean = false; // Toggle for showing graphs


  constructor(private backendService: BackendService, private router: Router) { }

  // Chart data and options
  barChartOptions: ChartOptions = {
    responsive: true,
    plugins: {
      title: {
        display: true,
        text: 'Vluchtstatus Verdeling',  // Example title — you can customize or pass dynamically if you want
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

  getFlightStatisticsByYear(): void {
    this.backendService.getData(`flights/statistics`).subscribe(
      (response) => {
        this.flightStatisticsByYear = response;
        console.log('Fetched flights:', this.flightStatisticsByYear);
      },
      (error) => {
        console.error('Error fetching flights:', error);
      }
    );
  }

  getFlightStatisticsByYearMonth(month: string = ''): void {
    this.backendService.getData(`flights/statistics/2024/${month}`).subscribe(
      (response) => {
        this.flightStatisticsByYearMonth = response;
        console.log('Fetched flights:', this.flightStatisticsByYearMonth);
      },
      (error) => {
        console.error('Error fetching flights:', error);
      }
    );
  }

  getFlightStatisticsByYearMonthDay(startDatetime: string = '', endDatetime: string = ''): void {
    this.backendService.getData(`flights/statistics?startDatetime=${startDatetime}&endDatetime=${endDatetime}`).subscribe(
      (response) => {
        this.flightStatisticsByYearMonthDay = response;
        console.log('Fetched flights:', this.flightStatisticsByYearMonth);
      },
      (error) => {
        console.error('Error fetching flights:', error);
      }
    );
  }

  getTouchPointStatisticsByYear(): void {
    this.backendService.getData(`touchpoints/statistics/yearly?year=2024`).subscribe(
      (response) => {
        this.touchPointStatisticsByYear = response;
        console.log('Fetched touchpoint statistics:', this.touchPointStatisticsByYear);
      },
      (error) => {
        console.error('Error fetching touchpoint statistics:', error);
      }
    )
  }

  getTouchPointStatisticsByYearMonth(month: string = ''): void {
    this.backendService.getData(`touchpoints/statistics/monthly?year=2024&month=${month}`).subscribe(
      (response) => {
        this.touchPointStatisticsByMonth = response;
        console.log('Fetched touchpoint statistics:', this.touchPointStatisticsByMonth);
      },
      (error) => {
        console.error('Error fetching touchpoint statistics:', error);
      }
    )
  }


  getFlightStatusChartData(statistics: FlightStatistics): ChartData<'doughnut'> {
    if (!statistics) {
      return { labels: [], datasets: [] };
    }
    return {
      labels: ['Vertraagd', 'Aangekomen', 'Omgeleid', 'Nachtvlucht'],
      datasets: [{
        data: [
          statistics.delayedFlights,
          statistics.arrivingFlights,
          statistics.divertedFlights,
          statistics.nachtvluchtFlights
        ],
        backgroundColor: ['#f54242', '#42f554', '#f5a142', '#4287f5']
      }]
    };
  }

  getEuNonEuChartData(statistics: FlightStatistics): ChartData<'doughnut'> {
    if (!statistics) {
      return { labels: [], datasets: [] };
    }
    return {
      labels: ['EU Vluchten', 'Niet EU Vluchten'],
      datasets: [{
        data: [statistics.euFlights, statistics.nonEuFlights],
        backgroundColor: ['#42f554', '#f54242']
      }]
    };
  }

  getPopularAirportsChartData(statistics: FlightStatistics): ChartData<'bar'> {
    if (!statistics || !statistics.mostPopularAirports) {
      return { labels: [], datasets: [] };
    }

    const labels = statistics.mostPopularAirports.map(a => a.airport);
    const data = statistics.mostPopularAirports.map(a => a.flightCount);

    return {
      labels,
      datasets: [{
        label: 'Aantal vluchten',
        data,
        backgroundColor: '#4287f5'
      }]
    };
  }

  getPassengerTypeChartData(statistics: FlightStatistics): ChartData<'doughnut'> {
    if (!statistics || !statistics.paxStatistics) {
      return { labels: [], datasets: [] };
    }

    const pax = statistics.paxStatistics;

    return {
      labels: ['Mannen', 'Vrouwen', 'Kinderen', 'Baby’s'],
      datasets: [{
        data: [pax.male, pax.female, pax.child, pax.infant],
        backgroundColor: ['#4287f5', '#f54291', '#f5a142', '#b542f5']
      }]
    };
  }

  getPassengerFlowChartData(statistics: FlightStatistics): ChartData<'doughnut'> {
    if (!statistics || !statistics.paxStatistics) {
      return { labels: [], datasets: [] };
    }

    const pax = statistics.paxStatistics;

    return {
      labels: ['Terminal passagiers', 'Transit passagiers'],
      datasets: [{
        data: [pax.terminal, pax.transit],
        backgroundColor: ['#42f554', '#f54242']
      }]
    };
  }

  getBaggageChartData(statistics: FlightStatistics): ChartData<'doughnut'> {
    if (!statistics || !statistics.baggageStatistics) {
      return { labels: [], datasets: [] };
    }

    const bags = statistics.baggageStatistics;

    return {
      labels: ['Terminal koffers', 'Transit koffers'],
      datasets: [{
        data: [bags.terminalBags, bags.transitBags],
        backgroundColor: ['#4287f5', '#f5a142']
      }]
    };
  }

  getTouchPointTopAirportsChartData(statistics: TouchPointStatisticsYearly): ChartData<'bar'> {
    if (!statistics || !statistics.topAirports || statistics.topAirports.length === 0) {
      return { labels: [], datasets: [] };
    }

    return {
      labels: statistics.topAirports.map(a => a.airport),
      datasets: [{
        label: 'Aantal vluchten',
        data: statistics.topAirports.map(a => a.count),
        backgroundColor: '#42a5f5'
      }]
    };
  }

  getTouchPointTopAircraftChartData(statistics: TouchPointStatisticsMonthly): ChartData<'bar'> {
    if (!statistics || !statistics.topAircraftTypes || statistics.topAircraftTypes.length === 0) {
      return { labels: [], datasets: [] };
    }

    // Since you only have an array of aircraft type strings, 
    // count occurrences of each type (in case duplicates exist)
    const typeCounts = statistics.topAircraftTypes.reduce((acc, type) => {
      acc[type] = (acc[type] || 0) + 1;
      return acc;
    }, {} as Record<string, number>);

    return {
      labels: Object.keys(typeCounts),
      datasets: [{
        label: 'Aantal vluchten',
        data: Object.values(typeCounts),
        backgroundColor: '#66bb6a'
      }]
    };
  }


  ngOnInit() {
    // this.generateDays(this.selectedYear, this.selectedMonth);
  }

  onYearSelected(year: number) {
    this.selectedYear = year;
    this.selectedMonth = null;
    this.days = [];
  }

  onMonthSelected(month: number) {
    this.selectedMonth = month;
    if (this.selectedYear && this.selectedMonth) {
      this.generateDays(this.selectedYear, this.selectedMonth);
    }
  }

  onDaySelected(day: number) {
    this.selectedDay = day;
  }

  generateDays(year: string | number | null, month: string | number | null) {

    if (year === null || month === null) {
      this.days = [];
      return;
    }

    const monthNumber = Number(month);
    this.days = [];
    let daysInMonth: number;

    switch (monthNumber) {
      case 2:
        daysInMonth = 29;
        break;
      case 4: case 6: case 9: case 11:
        daysInMonth = 30;
        break;
      default:
        daysInMonth = 31;
    }

    for (let day = 1; day <= daysInMonth; day++) {
      this.days.push(day);
    }
  }

  searchByYear() {
    console.log('Search by year:', this.selectedYear);
    this.getFlightStatisticsByYear();
  }
  searchByYearMonth() {
    console.log('Search by year and month:', this.selectedYear, this.selectedMonth);
    this.getFlightStatisticsByYearMonth(String(this.selectedMonth));
    // Add your search logic here
  }

  searchByYearMonthDay() {
    console.log('Search by year, month and day:', this.selectedYear, this.selectedMonth, this.selectedDay);

    if (this.selectedYear && this.selectedMonth && this.selectedDay) {
      const startDatetime = `${this.selectedYear}-${this.selectedMonth.toString().padStart(2, '0')}-${this.selectedDay.toString().padStart(2, '0')}T00:00:00`;
      const endDatetime = `${this.selectedYear}-${this.selectedMonth.toString().padStart(2, '0')}-${this.selectedDay.toString().padStart(2, '0')}T23:59:59`;

      this.getFlightStatisticsByYearMonthDay(startDatetime, endDatetime);
    } else {
      console.warn('Incomplete date selection.');
    }
  }

  searchByYearTouchPoint() {
    console.log('Search by year touch point:', this.selectedYear);
    this.getTouchPointStatisticsByYear();
  }

  searchByYearMonthTouchPoint() {
    console.log('Search by year and month touch point:', this.selectedYear, this.selectedMonth);
    this.getTouchPointStatisticsByYearMonth(String(this.selectedMonth));
  }

  clearFlightStatisticsByYear() {
    this.flightStatisticsByYear = null;
  }

  clearFlightStatisticsByYearMonth() {
    this.flightStatisticsByYearMonth = null;
  }

  clearFlightStatisticsByYearMonthDay() {
    this.flightStatisticsByYearMonthDay = null;
  }

  clearTouchPointStatisticsByYear() {
    this.touchPointStatisticsByYear = null;
  }

  clearTouchPointStatisticsByMonth() {
    this.touchPointStatisticsByMonth = null;
  }

}
