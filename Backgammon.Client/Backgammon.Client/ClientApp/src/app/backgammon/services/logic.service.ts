import { Injectable } from '@angular/core';
import { TwoNums } from 'src/app/models/TwoNums';
import {Cell} from "../../models/Cell";

@Injectable({
  providedIn: 'root'
})
export class LogicService {
  

  constructor() { }

  isAte(enemyPieces: number[], to: number):boolean {
      if(enemyPieces[to] > 1)
        return false;
      if(enemyPieces[to] == 1)
        return true;
      return false;
  }

  getTwoNumsFromRolls(rolls:number[]):TwoNums{
    if(rolls.length > 2) return {firstCube:rolls[0],secondCube:rolls[0]};
    if(rolls.length == 2) return {firstCube:rolls[0],secondCube:rolls[1]};
    if(rolls.length == 1) return {firstCube:rolls[0],secondCube:0};
    return {firstCube:0,secondCube:0}; 
  }

  getRemoveSteps(rolls:number[],from:number):number{
    let steps = 7;
    rolls.forEach(n=>{
      if(from+n > 24)
      {
        steps = Math.min(steps,n);
      }
    });
    if(steps == 7)
      return 0;
    return steps;
  }

  moveableFrom(nums:TwoNums,cellsWithMyPieces: number[], cellsWithOpPieces: number[]) :boolean[]{
    //making a commonCellsArray
    let cells:Cell[] = this.margeCells(cellsWithMyPieces,cellsWithOpPieces);

    let ret = cells.map((c,i)=>{
      if(!c.isMyPiece)
          return false;

      if(cellsWithMyPieces[0] > 0)
      {
          if(i != 0)
            return false;

          return this.canGetOut(nums,cellsWithOpPieces);
      }
      else
      {
        let moves = this.moveableToCheck(nums,cells,i);
      
        if(moves.firstCube == 0 && moves.secondCube == 0)
          return false

        return true;
      }
    });

    return ret;
  }

  private canGetOut(nums:TwoNums,cellsWithOpPieces:number[]):boolean
  {
    if(nums.firstCube != 0)
    {
      if(cellsWithOpPieces[nums.firstCube] < 2)
        return true;
    }

    if(nums.secondCube != 0)
    {
      if(cellsWithOpPieces[nums.secondCube] < 2)
        return true;
    }

    return false;
  }

  moveableTo(nums : TwoNums,  currentCell : number , cellsWithMyPieces: number[], cellsWithOpPieces: number[]):boolean[]{ //if player is eaten must give back 0 in current cell
    //making a commonCellsArray
    let cells:Cell[] = this.margeCells(cellsWithMyPieces,cellsWithOpPieces);

    let moves = this.moveableToCheck(nums,cells,currentCell);

    let movable = this.getMoveableTo(moves,currentCell);
    console.log(movable);
    return movable;
  }

  getAvailableRolls(nums:TwoNums):number[]{
    if(nums.firstCube == nums.secondCube)
    {
      return [nums.firstCube,nums.firstCube,nums.firstCube,nums.firstCube];
    }

    return [nums.firstCube,nums.secondCube];
  }

  removeRolls(rolls:number[],num:number):number[]{
    let removed = false;
    let res = rolls.filter(n=>{
      if(removed)
        return true;

      if(n == num)
      {
        removed = true;
        return false;
      }
      return true;
    });
    return res;
  }

  private moveableToCheck(nums:TwoNums,cells:Cell[],currentCell:number):TwoNums{
    let newNums = {firstCube:nums.firstCube,secondCube:nums.secondCube};
    let firstDestination =  nums.firstCube + currentCell;
    let secondDestination = nums.secondCube + currentCell;

    if(firstDestination == currentCell)
      newNums.firstCube = 0;
    else
    if(firstDestination < 25)
    {
      if(!(cells[firstDestination].isMyPiece || cells[firstDestination].numOfPieces <2 ))
        newNums.firstCube = 0;
    }
    else
    if(!this.isAbleToTakeOutPiece(currentCell, nums.firstCube, cells))
      newNums.firstCube = 0;


    if(secondDestination == currentCell)
      newNums.secondCube = 0
    else
    if(secondDestination < 25)
    {
      if(!(cells[secondDestination].isMyPiece || cells[secondDestination].numOfPieces <2 ))
        newNums.secondCube = 0;
    }
    else
    if(!this.isAbleToTakeOutPiece(currentCell, nums.secondCube, cells))
      newNums.secondCube = 0;

    return newNums;
  }

  private margeCells(cellsWithMyPieces: number[], cellsWithOpPieces: number[]){
    return cellsWithMyPieces.map((n,i)=>{
      if(n>0)
        return {numOfPieces:n,isMyPiece:true};
      
      if(cellsWithOpPieces[i] > 0)
        return {numOfPieces:cellsWithOpPieces[i],isMyPiece:false}

      return {numOfPieces:0,isMyPiece:false}
    });
  }

  private getMoveableTo(twoNums:TwoNums,current:number):boolean[]{
    let ret = [false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false];

    if(twoNums.firstCube > 0)
    {
      if(current + twoNums.firstCube > 24)
        ret[25] = true;
      else
        ret[current + twoNums.firstCube] = true;
    }

    if(twoNums.secondCube > 0)
    {
      if(current + twoNums.firstCube > 24)
        ret[25] = true;
      else
        ret[current + twoNums.secondCube] = true;
    }

    return ret;
  }

  private isAllPiecesAtTheEnd(cells: Cell[]):boolean{
    for (let index = 0; index < 19; index++) {
     if(cells[index].isMyPiece)
      return false;
    } 
     return true;    
  }

  private isAbleToTakeOutPiece(currentLocation:number , numOfSteps:number, cells : Cell[]):boolean
  {
    if(!this.isAllPiecesAtTheEnd(cells))
    return false;
    if(numOfSteps + currentLocation == 25)
      return true;
   
    for (let i = 19; i < currentLocation; i++) {
      if(cells[i].isMyPiece)
        return false;
    }
    return true;
  }
  

}
