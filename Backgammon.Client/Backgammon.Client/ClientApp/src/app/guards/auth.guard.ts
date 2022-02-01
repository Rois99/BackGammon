import { Injectable } from '@angular/core';
import {  CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService:AuthService,private router:Router) {
  }

  async canActivate(){
    if(this.authService.isLoggedIn())
    {
      this.router.navigate(['/lobby']);
    }
    return !this.authService.isLoggedIn();
  }
  
}
