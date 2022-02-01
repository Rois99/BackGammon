import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { catchError, filter, switchMap, take } from 'rxjs/operators';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  private isRefreshing = false;
  private refreshTokenSubject:BehaviorSubject<any>;
  constructor(public authService:AuthService) {
    this.refreshTokenSubject = new BehaviorSubject<any>(this.authService.getAccessToken());
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let token = this.authService.getAccessToken();
    if(token){
          request = this.addToken(request,token); 
    }

    return next.handle(request).pipe(catchError((error)=>{
      if(error instanceof HttpErrorResponse && error.status === 401){
        console.log("Try Refresh!");
        return this.handle401Error(request,next);
      }else{
        console.log("Failed Refresh!");
        return throwError(error);
      }
    }))
  }

  private addToken(request:HttpRequest<any>,token:string|null){
    return request.clone({
      setHeaders:{
        'Authorization': `Bearer ${token}`
      }
    });
  }

  private handle401Error(request:HttpRequest<any>,next:HttpHandler){
    if(!this.isRefreshing){
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((tokens:any)=>{
          this.isRefreshing = false;
          this.refreshTokenSubject.next(tokens);
          return next.handle(this.addToken(request,tokens.accessToken));
        }));
    }
    else{
      return this.refreshTokenSubject.pipe(
        filter(tokens=>tokens != null),
        take(1),
        switchMap(tokens=>{
          return next.handle(this.addToken(request,tokens.accessToken));
        })
      )
    }
  }
}
