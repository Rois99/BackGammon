import { Injectable } from '@angular/core';
import { Request } from 'src/app/models/Request';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { FirstMove } from 'src/app/contracts/FirstMove';
import { GameError } from 'src/app/contracts/GameError';
import { GameRequest } from 'src/app/contracts/GameRequest';
import { GameResult } from 'src/app/contracts/GameResult';
import { LastMove } from 'src/app/contracts/LastMove';
import { Move } from 'src/app/contracts/Move';
import { RequestResponse } from 'src/app/contracts/RequestResponse';
import { Player } from 'src/app/models/Player';
import { TwoNums } from 'src/app/models/TwoNums';
import { environment } from 'src/environments/environment';
import { TurnOver } from 'src/app/contracts/TurnOver';
import { HttpClient } from '@angular/common/http';
import {PlayerStats} from 'src/app/contracts/PlayerStats';

@Injectable({
  providedIn: 'root'
})
export class GameService {

  private readonly API = environment.api;
  connection?: signalR.HubConnection;
  connectedPlayers:BehaviorSubject<Player[]>;
  onGameError:BehaviorSubject<GameError|undefined>;
  onInvitationError:BehaviorSubject<GameError|undefined>;
  onGameRequest:BehaviorSubject<Request|undefined>;
  onGameRequestDenied:BehaviorSubject<Player|undefined>;
  onGameStart:BehaviorSubject<FirstMove|undefined>;
  onGameResult:BehaviorSubject<GameResult|undefined>;
  onTurnOver:BehaviorSubject<TurnOver|undefined>;
  onOpponentLastMove:BehaviorSubject<LastMove|undefined>;
  onOpponentMove:BehaviorSubject<Move|undefined>;

  private readonly HUBS = environment.hubs;


  constructor(private http: HttpClient) {
    this.onGameError = new BehaviorSubject<GameError|undefined>(undefined);
    this.onInvitationError = new BehaviorSubject<GameError|undefined>(undefined);
    this.onGameRequest = new BehaviorSubject<Request|undefined>(undefined);
    this.onGameRequestDenied = new BehaviorSubject<Player|undefined>(undefined);
    this.onGameStart = new BehaviorSubject<FirstMove|undefined>(undefined);
    this.onGameResult = new BehaviorSubject<GameResult|undefined>(undefined);
    this.onTurnOver = new BehaviorSubject<TurnOver|undefined>(undefined);
    this.onOpponentLastMove = new BehaviorSubject<LastMove|undefined>(undefined);
    this.onOpponentMove = new BehaviorSubject<Move|undefined>(undefined);
    this.connectedPlayers = new BehaviorSubject<Player[]>([]);
   }


  public initConnection(tokenFactory:()=>Promise<string>): Promise<void>{
    return new Promise(() => {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(this.HUBS.game,{accessTokenFactory:tokenFactory})
        .build();
  
      this.setSignalRClientMethods();

      this.connection
        .start()
        .then(() => {
          console.log(`Game SignalR connection success! connectionId: ${this.connection?.connectionId} `);
          this.sendPing();
        })
        .catch((error) => {
          console.log(`Game SignalR connection error: ${error}`);
        });
      
    });
  }

  public getPlayerStats(playerId:string){
    return this.http.get<PlayerStats>(`${this.API.game}/Stats/${playerId}`)
  }
  public closeConnection(){
    this.connection?.stop()
    .then(()=>{
      console.log(`Game SignalR connection stopped!`);
    });
  }

  private setSignalRClientMethods(){
    this.connection?.on("NewPlayerJoined",(player:Player)=>{
      this.connectedPlayers.next([...this.connectedPlayers.value,player]);   
    });

    this.connection?.on("Connected",(players:Player[])=>{
      this.connectedPlayers.next(players);
    });

    this.connection?.on("GameError",(errorNum:number)=>this.handleGameError(errorNum));

    this.connection?.on("PlayerDisconnected",(playerId:string)=>{
      let current = this.connectedPlayers.value;
      let next = current.filter(p=>p.id != playerId);
      this.connectedPlayers.next(next);
    });

    this.connection?.on("GameRequest",(request:GameRequest)=>{
      let current = this.connectedPlayers.value;
      let player = current.find(p=>p.id === request.senderId);
      if(player)
      {
        this.onGameRequest.next({player:player,requestId:request.requestId});
      }
    });

    this.connection?.on("RequestDenied",(receiverId:string)=>{
      let current = this.connectedPlayers.value;
      let receiver = current.find(p=>p.id === receiverId);
      if(receiver)
        this.onGameRequestDenied.next(receiver);
    });

    this.connection?.on("StartGame",(firstMove:FirstMove)=>{
        this.onGameStart.next(firstMove);
    });

    this.connection?.on("FinishGame",(result:GameResult)=>{
      this.onGameResult.next(result);
    });

    this.connection?.on("TurnIsOver",(turnOver:TurnOver)=>{
      this.onTurnOver.next(turnOver);
    });

    this.connection?.on("LastMoveOfOpponent",(lastMove:LastMove)=>{
      this.onOpponentLastMove.next(lastMove);
    }
    );

    this.connection?.on("OpponentMove",(move:Move)=>{
      this.onOpponentMove.next(move);
    });

    this.connection?.on("Pong",()=>{
      this.sendPing();
    });
  }

  private async sendPing(){
    await this.sleep(5000);
    await this.connection?.invoke("Ping");
  }

  private sleep(ms:number){
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  private handleGameError(error:number){
    switch (error) {
      case 1:
        alert("Player disconnected from game");
        console.log("Player disconnected from game");
        this.onGameError.next({message:"Opponent disconnected."})
        break;
      case 2:
        alert("you are in game");
        console.log("you are in game");
        this.onInvitationError.next({message:"You are in game"});
        break;
      case 3:
        alert("Opponent in game");
        this.onInvitationError.next({message:"Opponent in game"});
        break;
      case 4:
        alert("Sender unavailable");
        this.onInvitationError.next({message:"Sender unavailable"});
        // TODO
        break;
      case 5:
        alert("Illegal move");
        this.onGameError.next({message:"Illegal move"})
        // TODO
        break;
        case 6:
          alert("Player use cheats");
          this.onGameError.next({message:"Player use cheats"});
        // TODO
        break;
      default:
        break;
    }
  }

  async sendGameRequest(receiverId:string){
    await this.connection?.invoke("SendGameRequest",receiverId);
  }

  async sendGameRequestApproved(response:RequestResponse){
    await this.connection?.invoke("GameRequestApproved",response);
  }

  async sendMove(move:Move){
    await this.connection?.invoke("SendMove",move);
  }

}
