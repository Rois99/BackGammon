import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class LoginGuard implements CanActivate {
  constructor(private authService:AuthService,private router:Router) {
  }

  async canActivate(){
    if(!this.authService.isLoggedIn())
    {
      this.router.navigate(['/login-register']);
    }
    return this.authService.isLoggedIn();
  }
  
}
