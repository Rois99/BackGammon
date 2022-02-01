import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { LoginGuard } from './guards/login.guard';
import { LobbyComponent } from './components/lobby/lobby.component';
import { SettingsComponent } from './components/settings/settings.component';
import { LoginRegisterComponent } from './components/login-register/login-register.component';

const routes: Routes = [
  {path:'',pathMatch:'full',redirectTo:'/login-register'},
  {
    path:'login-register',
    component: LoginRegisterComponent,
    canActivate: [AuthGuard]
  },
  {
    path:'settings',
    component:SettingsComponent,
    canActivate: [LoginGuard]
  },
  {
    path:'lobby',
    component:LobbyComponent,
    canActivate: [LoginGuard]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
