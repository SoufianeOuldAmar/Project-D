import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BackendService {

  private apiUrl = 'http://localhost:5244/api/v1/';  // Update with your API endpoint

  constructor(private http: HttpClient) { }

  // Example: Fetch data from the backend
  getData(endpoint: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}${endpoint}`);
  }

}
