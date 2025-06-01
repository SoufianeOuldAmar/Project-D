import { Component } from '@angular/core';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-agenda',
  templateUrl: './agenda.component.html',
  imports: [NgFor],
  styleUrls: ['./agenda.component.css']
})
export class AgendaComponent {

  days: number[] = [];

  ngOnInit() {
    this.generateDays(2);
    console.log(this.days)
  }

  generateDays(month: string | number) {

    if (typeof month === 'number') {
      console.log(true)
    }
    else {
      console.log(false)
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


  onDayClick(day: number) {
    console.log('You clicked day', day);
  }
}
