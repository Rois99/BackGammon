import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {

  changeNameForm:FormGroup;
  currentUser?:string;

  constructor(private formBuilder:FormBuilder,private userService:UserService,private router:Router) { 
    this.changeNameForm =  this.formBuilder.group({
      username: ['']
    });
  }

  ngOnInit(): void {
    this.userService.getUsername()
    .subscribe(name=>this.currentUser = name);
  }

  change() {
    this.userService.setUsername(this.changeNameForm.controls['username'].value)
    .subscribe(name => {
      if (name) {
        this.currentUser = name;
        alert("Username changed successfully!")
      }
      else
      {
        alert("Failed changing username...")
      }
    });
  }

  routeToLobby(){
    this.router.navigate(['/lobby']);
  }
}
