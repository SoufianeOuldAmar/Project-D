import { Component } from '@angular/core';
import { NgFor, NgIf, CommonModule } from '@angular/common';
import { BackendService } from '../backend.service';
import { Router } from '@angular/router'; // ✅ Import Router


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




@Component({
  selector: 'app-agenda',
  templateUrl: './agenda.component.html',
  imports: [NgFor, NgIf, CommonModule],
  styleUrls: ['./agenda.component.css']
})
export class AgendaComponent {
  flightStatisticsByYear: FlightStatistics | null = null;
  flightStatisticsByYearMonth: FlightStatistics | null = null;
  flightStatisticsByYearMonthDay: FlightStatistics | null = null;
  selectedYear: number | null = null;
  selectedMonth: number | null = null;
  selectedDay: number | null = null;
  days: number[] = [];

  constructor(private backendService: BackendService, private router: Router) { }

  getFlightStatisticsByYear(): void {
    this.backendService.getData(`Statistics2024`).subscribe(
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
    this.backendService.getData(`monthly-statistics-2024?month=${month}`).subscribe(
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
    this.backendService.getData(`flight-statistics?startDatetime=${startDatetime}&endDatetime=${endDatetime}`).subscribe(
      (response) => {
        this.flightStatisticsByYearMonthDay = response;
        console.log('Fetched flights:', this.flightStatisticsByYearMonth);
      },
      (error) => {
        console.error('Error fetching flights:', error);
      }
    );
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

  clearFlightStatisticsByYear() {
    this.flightStatisticsByYear = null;
  }

  clearFlightStatisticsByYearMonth() {
    this.flightStatisticsByYearMonth = null;
  }

  clearFlightStatisticsByYearMonthDay() {
    this.flightStatisticsByYearMonthDay = null;
  }

}
