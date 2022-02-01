import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable, of } from 'rxjs';
import { catchError, map, mapTo, tap } from 'rxjs/operators';
import { User } from 'src/app/models/User';
import { environment } from 'src/environments/environment';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly API = environment.api;
  private currentUser?:User;
  private jwtHandler = new JwtHelperService();
  constructor(private http:HttpClient,private authService:AuthService) { }

  public setUsername(username:string){
    return this.http.patch<any>(`${this.API.identity}/user`,
                                {newUsername:username})
    .pipe(
      tap(()=>{
        if(this.currentUser)
          this.currentUser.name = username
      }),
      mapTo(username),
      catchError(error=>{
      console.log(error);
      return of(undefined);
    }))
  }

  public getUsername():Observable<string|undefined>{
    if(this.currentUser)
      return of(this.currentUser.name);
    return this.http.get<any>(`${this.API.identity}/user`)
    .pipe(
      tap(u=>this.setUser(u.username)),
      map<any,string>(u=>u.username),
      catchError(error=>{
        console.log(error);
        return of(undefined)
      }));
  }

  public getId():Observable<string|undefined>{
    if(this.currentUser)
      return of(this.currentUser.id);
      return this.getUsername()
      .pipe(
        map(u=>{
          if(u) return this.currentUser?.id;
          return undefined;
        }));
  }

  private setUser(name:string){
    let token = this.authService.getAccessToken();
    if(!token) return;
    let id = this.jwtHandler.decodeToken(token).sub;
    this.currentUser = {id:id,name:name};
  }
}
