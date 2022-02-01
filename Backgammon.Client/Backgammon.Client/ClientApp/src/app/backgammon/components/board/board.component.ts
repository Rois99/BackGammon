import { Component, OnInit, ChangeDetectorRef, Input } from '@angular/core';
import { PlayerStats } from 'src/app/contracts/PlayerStats';
import { StartGame } from 'src/app/models/StartGame';
import { GameService } from 'src/app/services/game/game.service';
import { GameBoardService } from '../../services/game-board.service';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.css']
})
export class BoardComponent implements OnInit {
  @Input() game:StartGame|undefined;

  readonly left_top = [12, 11, 10, 9, 8, 7];
  readonly left_bottom = [13, 14, 15, 16, 17, 18];
  readonly right_top = [6, 5, 4, 3, 2, 1];
  readonly right_bottom = [19, 20, 21, 22, 23, 24];

  moveable: boolean[] = [];
  playerTurn:boolean = false;
  firstRoll:boolean = false;
  moveableTo: boolean[] = [];
  blackPieces: number[] = [];
  whitePieces: number[] = [];
  dices: { dice1: number, dice2: number } | undefined;
  playerStats: PlayerStats | undefined;
  opponentStats: PlayerStats | undefined;


  constructor(private boardService: GameBoardService,private gameService:GameService) {

  }

  ngOnInit(): void {
    if(!this.game)
    return ;
    this.boardService.observeIfFirstRoll.subscribe(r=>this.firstRoll = r);
    this.boardService.initGame(this.game.playerColor,this.game.isStarting,this.game.whoIsFirstRoll,this.game.firstRoll,this.game.gameId);
    this.boardService.observeBlackPieces.subscribe(p => this.blackPieces = p);
    this.boardService.observeWhitePieces.subscribe(p => this.whitePieces = p);
    this.boardService.observeMoveable.subscribe(m => this.moveable = m);
    this.boardService.observeMoveableTo.subscribe(m =>{ this.moveableTo = m;});
    this.boardService.observeDices.subscribe(d => this.dices = d);
    this.boardService.observePlayerTurn.subscribe(t=>this.playerTurn = t);
    this.gameService.getPlayerStats(this.game.player.id).subscribe((stats)=> this.playerStats = stats, err=> console.log(err));
    this.gameService.getPlayerStats(this.game.opponent.id).subscribe((stats)=> this.opponentStats = stats, err=> console.log(err));

    
  }

  allowDrop(ev: DragEvent,i:number) {
    if(this.moveableTo[i])
      ev.preventDefault();
  }

  async drop(ev: DragEvent, toStack: number) {
    ev.preventDefault();
    this.boardService.dragStopped();
    let fromStack = ev.dataTransfer?.getData("fromStack");
    let pieceColor = ev.dataTransfer?.getData("color");
    if (fromStack && pieceColor) {
      let from = new Number(fromStack) as number;
      await this.boardService.playerMove(from, toStack);
    }
   
  }

  allowDropOut(ev: DragEvent) {
    if(this.moveableTo[25])
      ev.preventDefault();
  }

  async dropOut(ev: DragEvent) {
    ev.preventDefault();
    let fromStack = ev.dataTransfer?.getData("fromStack");
    if (fromStack) {
      let from = new Number(fromStack) as number;
      await this.boardService.playerRemovePice(from);
    }
  }

  dragStarted(from:number){
    console.log('Drag triggered!')
    this.boardService.dragStarted(from);
  }

  dragEnded(from:number){
    this.boardService.dragStopped();
  }


  // async rollDices() {
  //   await this.boardService.rollDices();
  // }
}
