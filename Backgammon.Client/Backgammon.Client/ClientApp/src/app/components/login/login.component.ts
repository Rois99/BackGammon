import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;

  constructor(private authService: AuthService, private formBuilder: FormBuilder, private router: Router) { 
    this.loginForm = this.formBuilder.group({
      username: [''],
      password: [''],
      keepMeLoggedIn:[true]
    });
  }

  ngOnInit() {
    
  }

  login() {
    this.authService.login(
      {
        username: this.loginForm.controls['username'].value,
        password: this.loginForm.controls['password'].value
      },
      this.loginForm.controls['keepMeLoggedIn'].value
    )
    .subscribe(success => {
      if (success) {
        this.router.navigate(['/lobby']);
      }
    });
  }

}
