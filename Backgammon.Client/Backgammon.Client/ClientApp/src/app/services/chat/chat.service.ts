import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { ChatterConnected } from 'src/app/contracts/ChatterConnected';
import { ChatterDisconnect } from 'src/app/contracts/ChatterDisconnect';
import { ConfirmMessage } from 'src/app/contracts/ConfirmMessage';
import { ConfirmRequest } from 'src/app/contracts/ConfirmRequest';
import { SendMessage } from 'src/app/contracts/SendMessage';
import { Chatter } from 'src/app/models/Chatter';
import { Message } from 'src/app/models/Message';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  connection?: signalR.HubConnection;
  private readonly HUBS = environment.hubs;
  private readonly APIS = environment.api;
  hubChatters:BehaviorSubject<Chatter[]>;
  incomingMessage:BehaviorSubject<Message|null>;
  incomingConfirmation:BehaviorSubject<ConfirmMessage|null>;

  constructor(private http:HttpClient) { 
    this.hubChatters = new BehaviorSubject<Chatter[]>([]);
    this.incomingMessage = new BehaviorSubject<Message|null>(null);
    this.incomingConfirmation = new BehaviorSubject<ConfirmMessage|null>(null);
  }

  public initConnection(tokenFactory:()=>Promise<string>): Promise<void>{
    return new Promise(() => {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(this.HUBS.chat,{accessTokenFactory:tokenFactory})
        .build();
  
      this.setSignalRClientMethods();

      this.connection
        .start()
        .then(() => {
          console.log(`SignalR connection success! connectionId: ${this.connection?.connectionId} `);
        })
        .catch((error) => {
          console.log(`SignalR connection error: ${error}`);
        });
    });
  }

  public closeConnection(){
    this.connection?.stop()
    .then(()=>{
      console.log(`SignalR connection stopped!`);
    });
  }

  private setSignalRClientMethods(){
    this.connection?.on('ChatterConnected',(c:ChatterConnected)=>{
      let current = this.hubChatters.value;
      let reconnected = current.find(cur=>cur.id == c.id);
      if(reconnected)
      {
        reconnected.isConnected = true;
        this.hubChatters.next(current);
      }else{
        let chatter:Chatter = {id:c.id,name:c.name,isConnected:true,lastSeen:undefined};
        this.hubChatters.next([...this.hubChatters.value,chatter]);
      }
    });

    this.connection?.on('SetChatters',(c:Chatter[])=>{
      this.hubChatters.next(c);
    });

    this.connection?.on('ChatterDisconnect',(c:ChatterDisconnect)=>{
      let current = this.hubChatters.value;
      let newList = current.flatMap(chatter=>{
        if(chatter.id === c.id)
          {
            chatter.isConnected = false;
            chatter.lastSeen = c.lastSeen;
          }

        return chatter;
      })
      this.hubChatters.next(newList);
    })

    this.connection?.on('SetMessage',(message:Message)=>{
      this.incomingMessage.next(message);
    });

    this.connection?.on('ConfirmMessage',(confirmation:ConfirmMessage)=>{
      this.incomingConfirmation.next(confirmation);
    });
  }

  public async sendMessage(message:SendMessage){
    await this.connection?.invoke('SendMessage',message);
  }

  public async confirmMessage(confirmation:ConfirmRequest){
    await this.connection?.invoke('ConfirmMessage',confirmation);
  }

  public getHistory(withId:string):Observable<Message[]>
  {
    return this.http.get<any>(`${this.APIS.chat}/history?chatWithId=${withId}`)
    .pipe(tap(mess=>console.log(mess)),map(mess=>mess.result));
  }
}
