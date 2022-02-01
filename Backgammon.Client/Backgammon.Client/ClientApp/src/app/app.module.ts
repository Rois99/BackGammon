import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { LoginComponent } from './components/login/login.component';
import { SettingsComponent } from './components/settings/settings.component';
import { LobbyComponent } from './components/lobby/lobby.component';
import { ReactiveFormsModule } from '@angular/forms';
import { TokenInterceptor } from './services/auth/token.interceptor';
import { RegisterComponent } from './components/register/register.component';
import { LoginRegisterComponent } from './components/login-register/login-register.component';
import { ChatComponent } from './components/chat/chat.component';
import { MessageComponent } from './components/message/message.component';
import { BoardComponent } from './backgammon/components/board/board.component';
import { StackComponent } from './backgammon/components/stack/stack.component';
import { PieceComponent } from './backgammon/components/piece/piece.component';
import { EnumeratePipe } from './pipes/enumerate.pipe';
import { DiceComponent } from './backgammon/components/dice/dice.component';
import { MaxPipe } from './pipes/max.pipe';
import { StackPipe } from './pipes/stack.pipe';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SettingsComponent,
    LobbyComponent,
    RegisterComponent,
    LoginRegisterComponent,
    ChatComponent,
    MessageComponent,
    BoardComponent,
    StackComponent,
    PieceComponent,
    EnumeratePipe,
    DiceComponent,
    MaxPipe,
    StackPipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule
  ],
  providers: [
    {
      provide:HTTP_INTERCEPTORS,
      useClass:TokenInterceptor,
      multi:true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
