import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login-register',
  templateUrl: './login-register.component.html',
  styleUrls: ['./login-register.component.css']
})
export class LoginRegisterComponent implements OnInit {

  isLogin =  true;

  constructor() { }

  ngOnInit(): void {
  }

  onRegisterClick(){
    this.isLogin = false;
  }

  onLoginClick(){
    this.isLogin = true;
  }
}
