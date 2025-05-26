import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root',
})
export class AuthService {

    private apiUrl = environment.apiUrl; // Make sure to set your backend API URL

    constructor(private http: HttpClient) { }

    login(credentials: { username: string, password: string }): Observable<any> {
        return this.http.post(`${this.apiUrl}/api/auth/login`, credentials);
    }

    register(userData: { username: string, email: string, password: string }): Observable<any> {
        return this.http.post(`${this.apiUrl}/api/auth/register`, userData);
    }
}
