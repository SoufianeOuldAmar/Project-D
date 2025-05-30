import { Injectable, inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  private router = inject(Router);

  canActivate(): boolean {
    if (typeof window !== 'undefined' && sessionStorage.getItem('accessToken')) {
      console.log('Access granted. Token found.', sessionStorage.getItem('accessToken'));
      return true;
    }

    this.router.navigate(['/login']);
    console.log('Access denied. Redirecting to login.');
    return false;
  }
}
