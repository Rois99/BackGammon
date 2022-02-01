import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map, mapTo, tap } from 'rxjs/operators';
import { Tokens } from 'src/app/models/Tokens';
import { environment } from 'src/environments/environment';
import { ChatService } from '../chat/chat.service';
import { GameService } from '../game/game.service';

@Injectable({
    providedIn: 'root'
  }
)
export class AuthService {

  private readonly API = environment.api;
  private readonly ACCESS_TOKEN = 'ACCESS_TOKEN';
  private readonly REFRESH_TOKEN = "REFRESH_TOKEN";
  private isChatServiceInit:boolean = false;
  private isGameServiceInit:boolean = false;

  constructor(private http: HttpClient,private chatService:ChatService,private gameService:GameService,private router:Router) { }

  login(user: { username: string, password: string }, keepLoggedIn: boolean): Observable<boolean> {
    return this.http.post<Tokens>(`${this.API.identity}/auth/login`, user)
      .pipe(
        tap((tokens: Tokens) => this.doLoginUser(tokens, keepLoggedIn)),
        mapTo(true),
        catchError(error => {
          console.log(error);
          return of(false);
        }));
  }

  public registerUser(user:{username:string,password:string,confirmPassword:string}, keepLoggedIn: boolean){
    return this.http.post<Tokens>(`${this.API.identity}/user/register`,user)
    .pipe(
      tap((tokens:Tokens)=>this.doLoginUser(tokens,keepLoggedIn)),
      mapTo(true),
      catchError(error=>{
        console.log(error);
        return of(false);
      }));
  }

  logout() {
    return this.http.post<any>(`${this.API.identity}/Auth/logout`,{
      'refreshToken': this.getRefreshToken()
    })
    .pipe(
      tap(() =>{
        console.log("Logging out!");
        this.doLogoutUser();
      }),
      mapTo(true),
      catchError(error => {
        console.log(error);
        return of(false);
      }));
  }

  refreshToken() {
    return this.http.post<Tokens>(`${this.API.identity}/auth/refresh`, {
      accessToken: this.getAccessToken(),
      refreshToken: this.getRefreshToken()
    }).pipe(tap((tokens: Tokens) => {
      this.reStoreTokens(tokens);
    }),catchError((error)=>{this.doLogoutUser();return error;}))
  }

  async accessTokenFactory(){
    let result = await this.http.get<any>(`${this.API.identity}/auth/ping`)
    .pipe(mapTo(true),
      catchError(error=>{
      console.log(error);
      return of(false);
    })).toPromise();

    if(!result)
    {
      this.doLogoutUser();
      return '';
    }

    let token = this.getAccessToken();

    if(!token)
    {
      this.doLogoutUser();
      return '';
    }
    return token; 
  }

  isLoggedIn(){
    let isTokens = !!this.getAccessToken();
    if(isTokens)
        this.initServices();    
      
    return isTokens;
  }

  getAccessToken() {
    return localStorage.getItem(this.ACCESS_TOKEN) ?? sessionStorage.getItem(this.ACCESS_TOKEN);
  }

  private doLoginUser(tokens: Tokens, keepLoggedIn: boolean) {
    this.storeTokens(tokens,!keepLoggedIn);
    this.initServices();
  }

  private doLogoutUser() {
    this.removeTokens();
    this.closeServices();
    this.router.navigate(['/login-register']);
  }

  private getRefreshToken() {
    let rToken = sessionStorage.getItem(this.REFRESH_TOKEN) ?? localStorage.getItem(this.REFRESH_TOKEN);
    return rToken;
  }

  private storeTokens(tokens: Tokens,isSession:boolean) {
    if (!isSession) {
      localStorage.setItem(this.ACCESS_TOKEN, tokens.accessToken);
      localStorage.setItem(this.REFRESH_TOKEN, tokens.refreshToken);
    }
    else {
      sessionStorage.setItem(this.ACCESS_TOKEN, tokens.accessToken);
      sessionStorage.setItem(this.REFRESH_TOKEN, tokens.refreshToken);
    }
  }

  private reStoreTokens(tokens: Tokens) {
    if (localStorage.getItem(this.ACCESS_TOKEN) &&
    localStorage.getItem(this.REFRESH_TOKEN)) {
      localStorage.setItem(this.ACCESS_TOKEN, tokens.accessToken);
      localStorage.setItem(this.REFRESH_TOKEN, tokens.refreshToken);
    }
    else {
      sessionStorage.setItem(this.ACCESS_TOKEN, tokens.accessToken);
      sessionStorage.setItem(this.REFRESH_TOKEN, tokens.refreshToken);
    }
  }

  private removeTokens() {

    localStorage.removeItem(this.ACCESS_TOKEN);
    localStorage.removeItem(this.REFRESH_TOKEN);

    sessionStorage.removeItem(this.ACCESS_TOKEN);
    sessionStorage.removeItem(this.REFRESH_TOKEN);

  }

  private initServices()
  {
    if(!this.isChatServiceInit)
    {
      this.chatService.initConnection(()=>this.accessTokenFactory());
      this.isChatServiceInit = true;
    }

    if(!this.isGameServiceInit)
    {
      this.gameService.initConnection(()=>this.accessTokenFactory());
      this.isGameServiceInit = true;
    }
  }

  private closeServices(){
    this.chatService.closeConnection();
    this.isChatServiceInit = false;

    this.gameService.closeConnection;
      this.isGameServiceInit = false;
  }
}