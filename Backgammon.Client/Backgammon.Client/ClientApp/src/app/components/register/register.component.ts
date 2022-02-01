import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  registerForm:FormGroup;

  constructor(private authService: AuthService, private formBuilder: FormBuilder, private router: Router) { 
    this.registerForm = this.formBuilder.group({
      username: [''],
      password: [''],
      confirmPassword: [''],
      keepMeLoggedIn:[true]
    });
  }

  ngOnInit(): void {
  }

  private getUsername = ():string=>this.registerForm.controls['username'].value;
  private getPassword = ():string=>this.registerForm.controls['password'].value;
  private getConfirmPassword = ():string=>this.registerForm.controls['confirmPassword'].value;
  private getKeepMeLoggedIn = ():boolean=>this.registerForm.controls['keepMeLoggedIn'].value;

  register(){
    this.authService.registerUser(
      {
        username:this.getUsername(),
        password:this.getPassword(),
        confirmPassword:this.getConfirmPassword()
      },this.getKeepMeLoggedIn())
      .subscribe(success=>{
        if(success)
          this.router.navigate(['/lobby']);
        else
          alert("failed to register");
      })
  }
}
