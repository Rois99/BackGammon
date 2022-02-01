import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Request } from 'src/app/models/Request';
import { FirstMove } from 'src/app/contracts/FirstMove';
import { Chatter } from 'src/app/models/Chatter';
import { StartGame } from 'src/app/models/StartGame';
import { Player } from 'src/app/models/Player';
import { AuthService } from 'src/app/services/auth/auth.service';
import { ChatService } from 'src/app/services/chat/chat.service';
import { GameService } from 'src/app/services/game/game.service';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.css']
})
export class LobbyComponent implements OnInit {

  currentUser?: string;
  currentChat?: Chatter;
  currentId?: string;
  chatters: Chatter[] = [];
  isPlaying: boolean = false;
  game: StartGame | undefined;
  howToOpen = false;
  infoOpen = false;
  connected: (all: Chatter[]) => Chatter[] = (all) => all.filter(c => c.isConnected);
  disconnected: (all: Chatter[]) => Chatter[] = (all) => all.filter(c => !c.isConnected);

  constructor(private userService: UserService, private router: Router, private chatService: ChatService, private authService: AuthService, private gameService: GameService) {

  }

  ngOnInit(): void {
    this.userService.getUsername()
      .subscribe(name => this.currentUser = name, error => console.log(error), () => console.log("completed"));
    this.userService.getId()
      .subscribe(id => this.currentId = id);
    this.chatService.hubChatters.subscribe((chatters: Chatter[]) => {
      this.chatters = chatters;
    });

    this.gameService.onGameRequest
      .subscribe(async (request: Request | undefined) => {
        if (request) {
          if (confirm(`Do you want to play with ${request.player.name}?`)) {
            await this.gameService.sendGameRequestApproved({ requestId: request.requestId, isAccepted: true });
          } else {
            await this.gameService.sendGameRequestApproved({ requestId: request.requestId, isAccepted: false });
          }
        }
      });

    this.gameService.onGameRequestDenied
      .subscribe((receiver: Player | undefined) => {
        if (receiver)
          alert(`${receiver.name} denied your invitation.`);
      });


    this.gameService.onGameStart
      .subscribe((firstMove: FirstMove | undefined) => {
        if (!firstMove)
          return;
        this.isPlaying = true;
        let isPlayerOne = firstMove.playerOne == this.currentId;

        let opponent = (isPlayerOne ? this.getChatter(firstMove.playerTwo) : this.getChatter(firstMove.playerOne)) ?? { id: "", isConnected: false, name: "" };
        this.game = {
          isStarting: false,
          playerColor: 'none',
          firstRoll: firstMove.playingCubes,
          whoIsFirstRoll: firstMove.whosFirstCubes,
          gameId: firstMove.gameId,
          opponent: opponent,
          player: {id: this.currentId ?? "", name: this.currentUser??"", isConnected :true}
        };
        if (isPlayerOne)
          if (firstMove.whosFirstCubes.firstCube > firstMove.whosFirstCubes.secondCube) {
            if(!this.game)
            return;
            this.game.isStarting = true;
            this.game.playerColor = 'white';
          }
          else {
            if(!this.game)
            return;
            this.game.isStarting = false;
            this.game.playerColor = 'white';
          }
        else
          if (firstMove.whosFirstCubes.secondCube > firstMove.whosFirstCubes.firstCube) {
            if(!this.game)
            return;
            this.game.isStarting = true;
            this.game.playerColor = 'black';
          } else {
            if(!this.game)
            return;
            this.game.isStarting = false;
            this.game.playerColor = 'black';
          }
      });

    this.gameService.onGameResult
      .subscribe((result) => {
        if (result) {
          this.isPlaying = false;
          this.game = undefined;

          let didYouWin = this.currentId == result.winnerId;
          if (didYouWin)
            alert("Congrats You Won The Game!");
          else {
            let otherName = this.getChatterName(result.winnerId);
            alert(`${otherName} Won The Game, You Lost :(`);
          }
        }
      });

    this.gameService.onGameError.subscribe((err)=>{
      this.game = undefined;
      this.isPlaying = false;
    })
  }

  routeToSettings() {
    this.router.navigate(['/settings']);
  }

  logout() {
    this.authService.logout()
      .subscribe((success) => {
        console.log(success);
      });
  }

  async inviteToGame(chatterId: string) {
    if (this.isPlaying)
      return;
    await this.gameService.sendGameRequest(chatterId);
  }

  openChat(currentChat: Chatter) {
    this.currentChat = currentChat;
  }

  private getChatterName(id: string) {
    return this.chatters.find(c => c.id == id)?.name;
  }
  private getChatter(id: string) {
    return this.chatters.find(c => c.id == id);
  }

  openHowTo(){
    this.infoOpen = false;
    this.howToOpen = !this.howToOpen;
  }

  closeHowTo(){
    this.howToOpen = false;
  }

  openInfo(){
    this.howToOpen = false;
    this.infoOpen = !this.infoOpen;
  }

  closeInfo(){
    this.infoOpen = false;
  }

}
