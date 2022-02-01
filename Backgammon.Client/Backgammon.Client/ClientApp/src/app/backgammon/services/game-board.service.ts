import { Injectable } from '@angular/core';
import { BehaviorSubject, delay, from } from 'rxjs';
import { LastMove } from 'src/app/contracts/LastMove';
import { Move } from 'src/app/contracts/Move';
import { TurnOver } from 'src/app/contracts/TurnOver';
import { TwoNums } from 'src/app/models/TwoNums';
import { GameService } from 'src/app/services/game/game.service';
import { AnimateDicesService } from './animate-dices.service';
import { AnimatePiecesService } from './animate-pieces.service';
import { LogicService } from './logic.service';
import { SoundService } from './sound.service';

@Injectable({
  providedIn: 'root'
})
export class GameBoardService {
  private isPlayerTurn:boolean = false;
  private playerColor:string = 'none';
  private gameId:string = '';

  private whitePieces = [0,0,0,0,0,0,0,0,0,0,0,0,5,0,0,0,0,3,0,5,0,0,0,0,0,0];
  private blackPieces = [0,2,0,0,0,0,5,0,3,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0];
  private playerMoveable = [false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false];
  private playerMoveableTo = [false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false];
  private dices:{dice1:number,dice2:number} = {dice1:0,dice2:0};
  private rolls:number[] = [];

  observeWhitePieces:BehaviorSubject<number[]> = new BehaviorSubject<number[]>(this.whitePieces);
  observeBlackPieces:BehaviorSubject<number[]> = new BehaviorSubject<number[]>(this.blackPieces);
  observeMoveable:BehaviorSubject<boolean[]> = new BehaviorSubject<boolean[]>(this.playerMoveable);
  observeMoveableTo:BehaviorSubject<boolean[]> = new BehaviorSubject<boolean[]>(this.playerMoveableTo);
  observePlayerTurn:BehaviorSubject<boolean>  = new BehaviorSubject<boolean>(this.isPlayerTurn);
  observeDices:BehaviorSubject<{dice1:number,dice2:number}> = new BehaviorSubject<{dice1:number,dice2:number}>(this.dices);
  observeIfFirstRoll:BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);

  constructor(private sound:SoundService,
    private animatePieces:AnimatePiecesService,
    private animateDices:AnimateDicesService,
    private logic:LogicService,
    private gameService:GameService) {

      gameService.onTurnOver.subscribe(d=>{
        if(d)
        { 
          this.doTurnOver(d)
        }
      });

      gameService.onOpponentMove.subscribe(m=>{
        if(m){
          this.doOpponentMove(m);
        }
      });

      gameService.onOpponentLastMove.subscribe(l=>{
        if(l)
        {
          this.opponentLastMove(l);
        }
      });


  }

  async initGame(playerColor:string,isStarting:boolean,whoIsFirstRoll:TwoNums,firstRoll:TwoNums,gameId:string){
    this.playerColor = playerColor;
    this.isPlayerTurn = isStarting;
    this.observePlayerTurn.next(this.isPlayerTurn);
    this.gameId = gameId;
    this.initBoard();

    await this.sleep(1000);

    this.observeDices.next({dice1:1,dice2:5});
    await this.startRollAnimation(whoIsFirstRoll);

    await this.sleep(3000);

    this.observeIfFirstRoll.next(false);

    await this.rollDices(firstRoll.firstCube,firstRoll.secondCube);

    if(this.isPlayerTurn)
    {
      this.rolls = this.logic.getAvailableRolls(firstRoll);
      this.playerMoveable = this.getAvailableMovesFromLogic();
      this.observeMoveable.next(this.playerMoveable);
    }   
  }
  private async startRollAnimation(whoIsFirstRoll: TwoNums) {
    let playerDice:number;
    let opDice:number;
    if(this.isPlayerTurn)
      if(whoIsFirstRoll.firstCube > whoIsFirstRoll.secondCube)
      {
        playerDice = whoIsFirstRoll.firstCube;
        opDice = whoIsFirstRoll.secondCube;
      }
      else 
      {
        playerDice = whoIsFirstRoll.secondCube;
        opDice = whoIsFirstRoll.firstCube;
      }
    else
    {
      if(whoIsFirstRoll.firstCube > whoIsFirstRoll.secondCube)
      {
        playerDice = whoIsFirstRoll.secondCube;
        opDice = whoIsFirstRoll.firstCube;
      }
      else 
      {
        playerDice = whoIsFirstRoll.firstCube;
        opDice = whoIsFirstRoll.secondCube;
      }
    }

    await this.rollDices(opDice,playerDice);
  }

  dragStarted(startedFrom:number){
    if(!this.isPlayerTurn)
      return;
    if(this.playerColor == 'white')
    {
      let twoNums = this.logic.getTwoNumsFromRolls(this.rolls);
      this.playerMoveableTo = this.logic.moveableTo(twoNums,
        startedFrom,
        this.whitePieces,
        this.blackPieces);
    }else{
      this.playerMoveableTo = this.logic.moveableTo(this.logic.getTwoNumsFromRolls(this.rolls),
        startedFrom,
        this.blackPieces,
        this.whitePieces);
    }
    this.observeMoveableTo.next(this.playerMoveableTo);
  }

  dragStopped(){
    this.setAllFalse(this.playerMoveableTo);
    this.observeMoveableTo.next(this.playerMoveableTo);
  }

  async rollDices(dice1:number,dice2:number) {
    // call server get random numbers
    //this.observeDices.next(undefined);
    this.dices = {dice1:dice1,dice2:dice2};  
    this.sound.playDiceRoll();
    await this.animateDices.animate(this.dices.dice1,this.dices.dice2);
    this.observeDices.next(this.dices);
  }

  private async opponentLastMove(l:LastMove){ 

    if(l.opponentMove.numOfSteps != 0)
      await this.doOpponentMove(l.opponentMove);
    else
      console.log("Skipped enemy move.");

    await this.rollDices(l.yourDices.firstCube,l.yourDices.secondCube)

    this.isPlayerTurn = true;
    this.observePlayerTurn.next(this.isPlayerTurn);
    this.rolls = this.logic.getAvailableRolls(l.yourDices);
    this.playerMoveable = this.getAvailableMovesFromLogic();
    this.observeMoveable.next(this.playerMoveable);
  }

  async playerMove(from:number,to:number){
    //if ok to move!
    let numOfSteps = to-from;

    this.setAllFalse(this.playerMoveable);
    this.observeMoveable.next(this.playerMoveable);

    if(this.logic.isAte(this.playerColor == 'white'?this.blackPieces:this.whitePieces,to))
    {
      await this.doMove(to,25,this.playerColor == 'white'?'black':'white');
    }
    
    await this.doMove(from,to,this.playerColor);

    this.rolls = this.logic.removeRolls(this.rolls,numOfSteps);
    this.playerMoveable = this.getAvailableMovesFromLogic();
    this.observeMoveable.next(this.playerMoveable);
    
    await this.gameService.sendMove({gameId:this.gameId,stackNumber:from,numOfSteps:numOfSteps});
  }

  async playerRemovePice(from:number){
    // check logic
    let numOfSteps = this.logic.getRemoveSteps(this.rolls,from);
    await this.doRemove(from,this.playerColor);
    //if ok to move!
    
    this.rolls = this.logic.removeRolls(this.rolls,numOfSteps);
    this.playerMoveable = this.getAvailableMovesFromLogic();
    this.observeMoveable.next(this.playerMoveable);

    await this.gameService.sendMove({gameId:this.gameId,stackNumber:from,numOfSteps:numOfSteps});
  } 

  private setAllFalse(list:boolean[]){
    for (let index = 0; index < list.length; index++) {
      list[index] = false;
    }
  }

  private async doRemove(from:number,player:string){
    this.dragStopped();
    if(player == 'white')
    {
      this.whitePieces[from] = this.whitePieces[from]-1;
      this.updateWhite();
    }else{
      this.blackPieces[from] = this.blackPieces[from]-1;
      this.updateBlack();
    }

    try{
      let animation = await this.animatePieces.removePiece(player,from);
    }catch (error){
      console.error(error);
    }

    this.sound.playPieceMove();
  }

  private async doMove(from:number,to:number,player:string){
    this.dragStopped();
    if(player == 'white')
    {
      this.whitePieces[from] = this.whitePieces[from]-1;
      this.updateWhite();
    }else{
      this.blackPieces[from] = this.blackPieces[from]-1;
      this.updateBlack();
    }

    if(from != to)
    try{
      let animation = await this.animatePieces.movePiece(player,from,to);
    }catch (error){
      console.error(error);
    }

    if(player == 'white'){
        this.whitePieces[to] = this.whitePieces[to]+1;
        this.updateWhite();
      }
      else{ 
      
        this.blackPieces[to] = this.blackPieces[to]+1;
        this.updateBlack();
      }  
  }

  private updateWhite(){
    this.observeWhitePieces.next(this.whitePieces);
  }

  private updateBlack(){
    this.observeBlackPieces.next(this.blackPieces);
  }

  private updateMoveable(){
    this.observeMoveable.next(this.playerMoveable);
    this.observeMoveableTo.next(this.playerMoveableTo);
  }

  private updateState(){
   this.updateWhite();
   this.updateBlack();
   this.updateMoveable();  
  }

  private getIndexes(array:number[]):number[]{
    let result:number[] = [];
    array.forEach((e,i)=>{
      if(e!=0)
        result.push(i)
    })
    return result;
  }

  private getMoveToIndexes(array:number[]):number[]{
    let result:number[] = [];
    array.forEach((e,i)=>{
      if(e==0)
        result.push(i);
    });
    return result;
  }

  private async doTurnOver(turnOver:TurnOver){

    if(turnOver.skipped)
      console.log("You have been skipped!");

    this.rolls = [];
    let nums = turnOver.newNums;
    this.isPlayerTurn = false;
    this.observePlayerTurn.next(this.isPlayerTurn);
    this.setAllFalse(this.playerMoveable);
    this.setAllFalse(this.playerMoveableTo);
    this.updateMoveable();

    await this.rollDices(nums.firstCube,nums.secondCube);
  }

  private sleep(ms:number){
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  private async doOpponentMove(move:Move){
    let to = move.stackNumber + move.numOfSteps;
    let opponent = this.playerColor == 'white' ? 'black' : 'white';

    if(to > 24 || to < 1)
      await this.doRemove(move.stackNumber,opponent)
    else
    {
      if(this.logic.isAte(opponent == 'white'?this.blackPieces:this.whitePieces,to))
      {
        await this.doMove(to,0,opponent == 'white'?'black':'white');
      }

      await this.doMove(move.stackNumber,to,opponent);
    }
  }

  private initBoard(){
    if(this.playerColor == 'white')
    {
      this.whitePieces = [0,2,0,0,0,0,0,0,0,0,0,0,5,0,0,0,0,3,0,5,0,0,0,0,0,0];
      this.blackPieces = [0,0,0,0,0,0,5,0,3,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0,2,0];
    }else{
      this.whitePieces = [0,0,0,0,0,0,5,0,3,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0,2,0];
      this.blackPieces = [0,2,0,0,0,0,0,0,0,0,0,0,5,0,0,0,0,3,0,5,0,0,0,0,0,0];
    }
      this.setAllFalse(this.playerMoveable);
      this.setAllFalse(this.playerMoveableTo);
      this.updateState();
  }

  private getAvailableMovesFromLogic(){
    return this.logic.moveableFrom(this.logic.getTwoNumsFromRolls(this.rolls),this.playerColor == 'white' ? this.whitePieces : this.blackPieces,this.playerColor == 'white' ? this.blackPieces : this.whitePieces);
  }
}
